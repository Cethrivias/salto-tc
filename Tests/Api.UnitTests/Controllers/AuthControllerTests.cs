using System.Threading.Tasks;
using Api.Controllers;
using Api.Models.Dtos;
using Api.Utils;
using Application.Services;
using Domain.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Api.UnitTests.Controllers {
  public class AuthControllerTests {
    [Fact]
    public async Task Login_WithInvalidCredentials_Returns401() {
      var credentials = new LoginRequestDto {
        Password = "test",
        Username = "test",
      };

      var serviceMock = new Mock<IUserService>();
      var jwtIssuer = new Mock<IJwtIssuer>();

      serviceMock.Setup(
        repo => repo.GetByCredentials(It.IsAny<string>(), It.IsAny<string>())
      ).ReturnsAsync((User) null);

      var controller = new AuthController(serviceMock.Object, jwtIssuer.Object);

      var response = await controller.Login(credentials);

      response.Result.Should().BeOfType<UnauthorizedResult>();
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsLoginResponseDto() {
      var credentials = new LoginRequestDto {
        Password = "test",
        Username = "test",
      };
      var user = new User {
        Id = 1,
        Password = credentials.Password,
        Username = credentials.Username
      };

      var serviceMock = new Mock<IUserService>();
      var jwtIssuer = new Mock<IJwtIssuer>();

      serviceMock.Setup(
        repo => repo.GetByCredentials(credentials.Username, credentials.Password)
      ).ReturnsAsync(user);

      var controller = new AuthController(serviceMock.Object, jwtIssuer.Object);

      var response = await controller.Login(credentials);

      response.Result.Should().BeOfType<OkObjectResult>();
      var value = response.GetObjectResult();
      value.Should().BeOfType<LoginResponseDto>();
    }
  }
}