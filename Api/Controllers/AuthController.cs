using System;
using System.Threading.Tasks;
using Api.Core;
using Api.Models.Dtos;
using Api.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers {
  [ApiController]
  [Route("api/auth")]
  [Produces("application/json")]
  public class AuthController : ControllerBase {
    private IJwtIssuer jwtIssuer;
    private IUserRepository userRepository;

    public AuthController(IUserRepository userRepository, IJwtIssuer jwtIssuer) {
      this.userRepository = userRepository;
      this.jwtIssuer = jwtIssuer;
    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto credentials) {
      var user = await userRepository.GetByCredentials(credentials);

      if (user is null) {
        return Unauthorized();
      }

      var token = jwtIssuer.WriteToken(user);
      var response = new LoginResponseDto(token);

      return response;
    }
  }
}