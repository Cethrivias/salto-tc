using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Models.Dtos;
using Api.Utils;
using Api.Utils.Extensions;
using Application.Core;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers {

  [ApiController]
  [Route("api/locks")]
  [Produces("application/json")]
  [Authorize]
  public class LockController : ControllerBase {
    private readonly IUserService userService;
    private readonly IUserAccessLogService userAccessLogService;
    private readonly ILockOpener lockOpener;
    private readonly ILockService lockService;
    private readonly IUserProvider userProvider;

    public LockController(
      ILockOpener lockOpener,
      ILockService lockService,
      IUserService userService,
      IUserAccessLogService userAccessLogService,
      IUserProvider userProvider
    ) {
      this.userService = userService;
      this.userAccessLogService = userAccessLogService;
      this.lockOpener = lockOpener;
      this.lockService = lockService;
      this.userProvider = userProvider;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<LockDto>>> Get() {
      var userId = userProvider.UserId;

      var locks = await lockService.GetAccessibleByUser(userId);

      return Ok(locks.Select(it => it.ToLockDto()).ToList());
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<LockDto>> GetById([FromRoute(Name = "id")] int lockId) {
      var userId = userProvider.UserId;

      var item = await lockService.GetAccessibleByUser(userId, lockId);
      if (item is null) {
        return Forbid();
      }

      return Ok(item.ToLockDto());
    }

    [HttpGet("{id}/open")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<UserAccessLogDto>> Open([FromRoute(Name = "id")] int lockId) {
      var userId = userProvider.UserId;

      var hasAccess = await userService.HasAccess(userId, lockId);

      if (hasAccess) {
        await lockOpener.Open(lockId);
        var userAccessLog = await userAccessLogService.LogUserAccess(userId, lockId);
        return Ok(userAccessLog.ToUserAccessLogDto());
      } else {
        return Forbid();
      }
    }
  }
}