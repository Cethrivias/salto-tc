using Api.Core;
using Api.Models.Dtos;
using Api.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Api.Controllers {
  [ApiController]
  [Route("api/auth")]
  [Produces("application/json")]
  public class AuthController : ControllerBase {
    private JwtIssuer jwtIssuer;
    private IUserRepository userRepository;

    public AuthController(IUserRepository userRepository, IConfiguration configuration) {
      this.userRepository = userRepository;
      this.jwtIssuer = new(configuration);
    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<LoginResponseDto> Login([FromBody] LoginRequestDto credentials) {
      var user = userRepository.GetByCredentials(credentials);

      if (user is null) {
        return Unauthorized();
      }

      var token = jwtIssuer.WriteToken(user);
      var response = new LoginResponseDto(token);

      return response;
    }
  }
}