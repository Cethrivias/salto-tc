using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Api.Core.Database;
using Api.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers {

  [ApiController]
  [Route("api/lock")]
  [Produces("application/json")]
  [Authorize]
  public class LockController : ControllerBase {
    private readonly IUserRepository userRepository;

    public LockController(IUserRepository userRepository) {
      this.userRepository = userRepository;
    }

    [HttpGet("{lockId}/open")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> Open(int lockId) {
      var nameIdentifier = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
      var userId = Int32.Parse(nameIdentifier);
      var hasAccess = await userRepository.HasAccess(userId, lockId);

      if (hasAccess) {
        return Ok("Open");
      } else {
        return Unauthorized();
      }
    }
  }
}