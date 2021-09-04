using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Core;
using Api.Models.Dtos;
using Api.Repositories;
using Api.Utils;
using Api.Utils.Extentions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers {

  [ApiController]
  [Route("api/locks")]
  [Produces("application/json")]
  [Authorize]
  public class LockController : ControllerBase {
    private readonly IUserRepository userRepository;
    private readonly IUserAccessLogRepository userAccessLogRepository;
    private readonly ILockOpener lockOpener;
    private readonly ILockRepository lockRepository;
    private readonly IUserProvider userProvider;

    public LockController(
      ILockOpener lockOpener,
      ILockRepository lockRepository,
      IUserRepository userRepository,
      IUserAccessLogRepository userAccessLogRepository,
      IUserProvider userProvider
    ) {
      this.userRepository = userRepository;
      this.userAccessLogRepository = userAccessLogRepository;
      this.lockOpener = lockOpener;
      this.lockRepository = lockRepository;
      this.userProvider = userProvider;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<LockDto>>> Get() {
      var userId = userProvider.UserId;

      var locks = await lockRepository.GetAccessibleByUser(userId);

      return Ok(locks.Select(it => it.ToLockDto()).ToList());
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<LockDto>> GetById([FromRoute(Name = "id")] int lockId) {
      var userId = userProvider.UserId;

      var item = await lockRepository.GetAccessibleByUser(userId, lockId);
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

      var hasAccess = await userRepository.HasAccess(userId, lockId);

      if (hasAccess) {
        await lockOpener.Open(lockId);
        var userAccessLog = await userAccessLogRepository.LogUserAccess(userId, lockId);
        return Ok(userAccessLog.ToUserAccessLogDto());
      } else {
        return Forbid();
      }
    }
  }
}