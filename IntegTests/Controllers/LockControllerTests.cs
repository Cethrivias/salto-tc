using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Api;
using Api.Libs.Database;
using Api.Models;
using Api.Models.Dtos;
using Api.Utils;
using Api.Utils.Extentions;
using FluentAssertions;
using FluentAssertions.Extensions;
using IntegTests.Tools;
using LinqToDB;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace IntegTests.Controllers {
  public class LockControllerSetupFixture : IDisposable {
    private readonly MainDataConnection db = Database.GetConnection();

    public readonly User User = new() {
      Username = "Lock Test User 1",
      Password = new LoginRequestDto { Password = "12345" }.Password
    };
    public readonly Tag Tag = new() { Name = "Lock Test Tag 1" };
    public readonly Lock Lock = new() { Name = "Lock Test Lock 1" };

    public LockControllerSetupFixture() {
      var logsIds = (from logs in db.UsersAccessLogs
                     join users in db.Users on logs.UserId equals users.Id
                     where users.Username == User.Username
                     select logs.Id).ToHashSet();
      db.UsersAccessLogs.Where(it => logsIds.Contains(it.Id)).Delete();
      db.Users.Where(it => it.Username == User.Username).Delete();
      db.Tags.Where(it => it.Name == Tag.Name).Delete();
      db.Locks.Where(it => it.Name == Lock.Name).Delete();

      User.TagId = Tag.Id = db.InsertWithInt32Identity(Tag);
      Lock.Id = db.InsertWithInt32Identity(Lock);
      User.Id = db.InsertWithInt32Identity(User);
      db.Insert(new Access { LockId = Lock.Id, TagId = Tag.Id });
    }

    public void Dispose() {
      db.UsersAccessLogs.Where(it => it.UserId == User.Id).Delete();
      db.Delete(User);
      db.Delete(Tag);
      db.Delete(Lock);
      db.Close();
    }
  }

  public class LockControllerTests : IClassFixture<WebApplicationFactory<Startup>>, IClassFixture<LockControllerSetupFixture> {
    private readonly WebApplicationFactory<Startup> factory;
    private readonly LockControllerSetupFixture setup;

    public LockControllerTests(
      WebApplicationFactory<Startup> factory,
      LockControllerSetupFixture setup
    ) {
      this.factory = factory;
      this.setup = setup;
    }

    [Fact]
    public async Task Get_ReturnsAvailableLocks() {
      var client = factory.CreateClient();
      var token = new JwtIssuer(Config.GetInstance()).WriteToken(setup.User);
      var msg = new HttpRequestMessage(HttpMethod.Get, "api/locks");
      msg.Headers.Add("Authorization", $"Bearer {token}");

      var response = await client.SendAsync(msg);

      response.EnsureSuccessStatusCode();
      var responseBody = await Json.DeserializeResponse<List<LockDto>>(response);
      responseBody.Should().ContainSingle();
      responseBody.Should().ContainEquivalentOf(setup.Lock.ToLockDto());
    }

    [Fact]
    public async Task GetById_WhenUserHasAccess_ReturnsLock() {
      var client = factory.CreateClient();
      var token = new JwtIssuer(Config.GetInstance()).WriteToken(setup.User);
      var msg = new HttpRequestMessage(HttpMethod.Get, $"api/locks/{setup.Lock.Id}");
      msg.Headers.Add("Authorization", $"Bearer {token}");

      var response = await client.SendAsync(msg);

      response.EnsureSuccessStatusCode();
      var responseBody = await Json.DeserializeResponse<LockDto>(response);
      responseBody.Should().BeEquivalentTo(setup.Lock.ToLockDto());
    }

    [Fact]
    public async Task GetById_WhenUserDoesNotHaveAccess_Returns403() {
      var client = factory.CreateClient();
      var token = new JwtIssuer(Config.GetInstance()).WriteToken(setup.User);
      var msg = new HttpRequestMessage(HttpMethod.Get, $"api/locks/{Int32.MaxValue}");
      msg.Headers.Add("Authorization", $"Bearer {token}");

      var response = await client.SendAsync(msg);

      response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Open_WhenUserHasAccess_ReturnsUserAccessLogDto() {
      var client = factory.CreateClient();
      var token = new JwtIssuer(Config.GetInstance()).WriteToken(setup.User);
      var msg = new HttpRequestMessage(HttpMethod.Get, $"api/locks/{setup.Lock.Id}/open");
      msg.Headers.Add("Authorization", $"Bearer {token}");

      var response = await client.SendAsync(msg);

      response.EnsureSuccessStatusCode();
      var responseBody = await Json.DeserializeResponse<UserAccessLogDto>(response);
      responseBody.Id.Should().BeGreaterThan(0);
      responseBody.LockId.Should().Be(setup.Lock.Id);
      responseBody.CreatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, 5.Seconds());
    }

    [Fact]
    public async Task Open_WhenUserDoesNotHaveAccess_Returns403() {
      var client = factory.CreateClient();
      var token = new JwtIssuer(Config.GetInstance()).WriteToken(setup.User);
      var msg = new HttpRequestMessage(HttpMethod.Get, $"api/locks/{Int32.MaxValue}/open");
      msg.Headers.Add("Authorization", $"Bearer {token}");

      var response = await client.SendAsync(msg);

      response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
  }
}