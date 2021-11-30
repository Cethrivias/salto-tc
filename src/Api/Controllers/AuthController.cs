using System.Threading.Tasks;
using Api.Models.Dtos;
using Api.Utils;
using Application.Mediatr.User;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
  [ApiController]
  [Route("api/auth")]
  [Produces("application/json")]
  public class AuthController : ControllerBase
  {
    private readonly IJwtIssuer _jwtIssuer;
    private readonly IMediator _mediator; 

    public AuthController(IJwtIssuer jwtIssuer, IMediator mediator)
    {
      _jwtIssuer = jwtIssuer;
      _mediator = mediator;
    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto credentials)
    {
      var query = new GetUserByCredentialsQuery
      {
        Username = credentials.Username,
        Password = credentials.Password,
      };

      var user = await _mediator.Send(query);

      if (user is null) {
        return Unauthorized();
      }

      var token = _jwtIssuer.WriteToken(user);
      var response = new LoginResponseDto(token);

      return Ok(response);
    }
  }
}