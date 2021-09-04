using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Models;
using Api.Repositories;
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
    private readonly ILockRepository lockRepository;

    public LockController(IUserRepository userRepository, IUserAccessLogRepository userAccessLogRepository, ILockRepository lockRepository) {
      this.userRepository = userRepository;
      this.userAccessLogRepository = userAccessLogRepository;
      this.lockRepository = lockRepository;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<Lock>>> Get() {
      var userId = HttpContext.GetUserId();

      var locks = await lockRepository.GetAccessibleByUser(userId);

      return Ok(locks);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<Lock>> GetById([FromRoute(Name = "id")] int lockId) {
      var userId = HttpContext.GetUserId();

      var item = await lockRepository.GetAccessibleByUser(userId, lockId);
      if (item is null) {
        return Forbid();
      }

      return Ok(item);
    }

    [HttpGet("{id}/open")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<UserAccessLog>> Open([FromRoute(Name = "id")] int lockId) {
      var userId = HttpContext.GetUserId();

      var hasAccess = await userRepository.HasAccess(userId, lockId);

      if (hasAccess) {
        var userAccessLog = await userAccessLogRepository.LogUserAccess(userId, lockId);
        return Ok(userAccessLog);
      } else {
        return Forbid();
      }
    }
  }
}