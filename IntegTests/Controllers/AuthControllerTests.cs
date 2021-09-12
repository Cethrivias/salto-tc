using System.Net;
using System;
using System.Linq;
using System.Threading.Tasks;
using Api;
using Api.Libs.Database;
using Api.Models;
using Api.Models.Dtos;
using FluentAssertions;
using IntegTests.Tools;
using LinqToDB;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace IntegTests.Controllers {
  public class AuthControllerSetupFixture : IDisposable {
    private readonly MainDataConnection db = Database.GetConnection();

    public AuthControllerSetupFixture() {
      var user = new User {
        Username = "Auth Test User 1",
        Password = new LoginRequestDto { Password = "12345" }.Password
      };
      db.Insert(user);
    }

    public void Dispose() {
      db.Users.Where(it => it.Username == "Auth Test User 1").Delete();
      db.Close();
    }
  }

  public class AuthControllerTests : IClassFixture<WebApplicationFactory<Startup>>,
    IClassFixture<AuthControllerSetupFixture> {
    private readonly WebApplicationFactory<Startup> factory;

    public AuthControllerTests(
      WebApplicationFactory<Startup> factory
    ) {
      this.factory = factory;
    }

    [Fact]
    public async Task Login_WhenInvalidCredentials_Returns401() {
      var client = factory.CreateClient();
      var requestBody = Json.SerializeRequest(
        new {
          Password = "invalid password",
          Username = "Auth Test User 1"
        }
      );

      var response = await client.PostAsync("api/auth/login", requestBody);

      response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
      var responseBody = await Json.DeserializeResponse<LoginResponseDto>(response);
      responseBody.Token.Should().BeNullOrEmpty();
    }

    [Fact]
    public async Task Login_WhenValidCredentials_ReturnsToken() {
      var client = factory.CreateClient();
      var requestBody = Json.SerializeRequest(
        new {
          Password = "12345",
          Username = "Auth Test User 1"
        }
      );

      var response = await client.PostAsync("api/auth/login", requestBody);

      response.EnsureSuccessStatusCode();
      var responseBody = await Json.DeserializeResponse<LoginResponseDto>(response);
      responseBody.Token.Should().NotBeNullOrEmpty();
    }
  }
}