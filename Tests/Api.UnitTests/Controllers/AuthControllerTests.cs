using System.Threading;
using System.Threading.Tasks;
using Api.Controllers;
using Api.Models.Dtos;
using Api.Utils;
using Application.Mediatr.User;
using Domain.Models;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Api.UnitTests.Controllers
{
  public class AuthControllerTests
  {
    private readonly Mock<IJwtIssuer> _jwtIssuer;
    private readonly Mock<IMediator> _mediator;
    private readonly AuthController _sut;

    public AuthControllerTests()
    {
      _jwtIssuer = new Mock<IJwtIssuer>();
      _mediator = new Mock<IMediator>();
      _sut = new AuthController(_jwtIssuer.Object, _mediator.Object);
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_Returns401()
    {
      var credentials = new LoginRequestDto
      {
        Password = "test",
        Username = "test",
      };

      _mediator.Setup(x => x.Send(It.IsAny<GetUserByCredentialsQuery>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync((User) null);

      var response = await _sut.Login(credentials);

      response.Result.Should().BeOfType<UnauthorizedResult>();
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsLoginResponseDto()
    {
      var credentials = new LoginRequestDto
      {
        Password = "test",
        Username = "test",
      };
      var user = new User
      {
        Id = 1,
        Password = credentials.Password,
        Username = credentials.Username
      };

      _mediator.Setup(
        x => x.Send(
          It.Is<GetUserByCredentialsQuery>(
            q => q.Username == credentials.Username && q.Password == credentials.Password
          ), It.IsAny<CancellationToken>()
        )
      ).ReturnsAsync(user);

      var response = await _sut.Login(credentials);

      response.Result.Should().BeOfType<OkObjectResult>();
      var value = response.GetObjectResult();
      value.Should().BeOfType<LoginResponseDto>();
    }
  }
}