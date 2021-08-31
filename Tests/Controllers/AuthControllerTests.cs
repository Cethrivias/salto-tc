using System;
using Api.Controllers;
using Api.Models;
using Api.Models.Dtos;
using Api.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Tests.Controllers {
  public class AuthControllerTests {
    [Fact]
    public void Login_WithInvalidCredentials_Returns401() {
      var credentials = new LoginRequestDto {
        Password = "test",
        Username = "test",
      };

      var repoMock = new Mock<IUserRepository>();
      repoMock.Setup(repo => repo.GetByCredentials(It.IsAny<LoginRequestDto>())).Returns((User)null);

      var controller = new AuthController(repoMock.Object);

      var result = controller.Login(credentials);

      Assert.IsType<UnauthorizedResult>(result.Result);
    }

    [Fact]
    public void Login_WithValidCredentials_ReturnsLoginResponseDto() {
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
      repoMock.Setup(repo => repo.GetByCredentials(credentials)).Returns(user);

      var controller = new AuthController(repoMock.Object);

      var result = controller.Login(credentials);

      Assert.IsType<LoginResponseDto>(result.Value);
    }
  }
}