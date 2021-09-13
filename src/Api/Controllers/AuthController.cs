using System.Threading.Tasks;
using Api.Models.Dtos;
using Api.Utils;
using Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers {
  [ApiController]
  [Route("api/auth")]
  [Produces("application/json")]
  public class AuthController : ControllerBase {
    private readonly IJwtIssuer jwtIssuer;
    private readonly IUserService userService;

    public AuthController(IUserService userService, IJwtIssuer jwtIssuer) {
      this.userService = userService;
      this.jwtIssuer = jwtIssuer;
    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto credentials) {
      var user = await userService.GetByCredentials(credentials.Username, credentials.Password);

      if (user is null) {
        return Unauthorized();
      }

      var token = jwtIssuer.WriteToken(user);
      var response = new LoginResponseDto(token);

      return Ok(response);
    }
  }
}