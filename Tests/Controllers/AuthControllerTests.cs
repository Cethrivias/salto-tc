using System.Threading.Tasks;
using Api.Controllers;
using Api.Core;
using Api.Models;
using Api.Models.Dtos;
using Api.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Tests.Controllers {
  public class AuthControllerTests {
    [Fact]
    public async Task Login_WithInvalidCredentials_Returns401() {
      var credentials = new LoginRequestDto {
        Password = "test",
        Username = "test",
      };

      var repoMock = new Mock<IUserRepository>();
      var jwtIssuer = new Mock<IJwtIssuer>();

      repoMock.Setup(repo => repo.GetByCredentials(It.IsAny<LoginRequestDto>())).ReturnsAsync((User)null);

      var controller = new AuthController(repoMock.Object, jwtIssuer.Object);

      var result = await controller.Login(credentials);

      Assert.IsType<UnauthorizedResult>(result.Result);
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

      var repoMock = new Mock<IUserRepository>();
      var jwtIssuer = new Mock<IJwtIssuer>();

      repoMock.Setup(repo => repo.GetByCredentials(credentials)).ReturnsAsync(user);

      var controller = new AuthController(repoMock.Object, jwtIssuer.Object);

      var result = await controller.Login(credentials);

      Assert.IsType<LoginResponseDto>(result.Value);
    }
  }
}