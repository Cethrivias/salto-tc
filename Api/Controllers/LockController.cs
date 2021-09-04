using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Api.Models;
using Api.Repositories;
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

    public LockController(IUserRepository userRepository, IUserAccessLogRepository userAccessLogRepository) {
      this.userRepository = userRepository;
      this.userAccessLogRepository = userAccessLogRepository;
    }

    [HttpGet("{lockId}/open")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserAccessLog>> Open(int lockId) {
      var nameIdentifier = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
      var userId = Int32.Parse(nameIdentifier);

      var hasAccess = await userRepository.HasAccess(userId, lockId);

      if (hasAccess) {
        var userAccessLog = await userAccessLogRepository.LogUserAccess(userId, lockId);
        return Ok(userAccessLog);
      } else {
        return Unauthorized();
      }
    }
  }
}