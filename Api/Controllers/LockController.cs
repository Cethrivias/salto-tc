using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers {

  [ApiController]
  [Route("api/lock")]
  [Produces("application/json")]
  [Authorize]
  public class LockController : ControllerBase {

    [HttpGet("open")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult Open() {
      return Ok("Open");
    }
  }
}