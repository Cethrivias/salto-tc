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

    public User user = new User {
      Username = "Lock Test User 1",
      Password = new LoginRequestDto { Password = "12345" }.Password
    };
    public Tag tag = new Tag { Name = "Lock Test Tag 1" };
    public Lock _lock = new Lock { Name = "Lock Test Lock 1" };

    public LockControllerSetupFixture() {
      var logsIds = (from logs in db.UsersAccessLogs
                     join users in db.Users on logs.UserId equals users.Id
                     select logs.Id).ToHashSet();
      db.UsersAccessLogs.Where(it => logsIds.Contains(it.Id)).Delete();
      db.Users.Where(it => it.Username == user.Username).Delete();
      db.Tags.Where(it => it.Name == tag.Name).Delete();
      db.Locks.Where(it => it.Name == _lock.Name).Delete();

      user.TagId = tag.Id = db.InsertWithInt32Identity(tag);
      _lock.Id = db.InsertWithInt32Identity(_lock);
      user.Id = db.InsertWithInt32Identity(user);
      db.Insert(new Access { LockId = _lock.Id, TagId = tag.Id });
    }

    public void Dispose() {
      db.UsersAccessLogs.Where(it => it.UserId == user.Id).Delete();
      db.Delete(user);
      db.Delete(tag);
      db.Delete(_lock);
      db.Close();
    }
  }

  public class LockControllerTests : IClassFixture<WebApplicationFactory<Api.Startup>>, IClassFixture<LockControllerSetupFixture> {
    private readonly WebApplicationFactory<Startup> factory;
    private readonly LockControllerSetupFixture setup;

    public LockControllerTests(
      WebApplicationFactory<Api.Startup> factory,
      LockControllerSetupFixture setup
    ) {
      this.factory = factory;
      this.setup = setup;
    }

    [Fact]
    public async Task Get_ReturnsAvailableLocks() {
      var client = factory.CreateClient();
      var token = new JwtIssuer(Config.GetInstance()).WriteToken(setup.user);
      var msg = new HttpRequestMessage(HttpMethod.Get, "api/locks");
      msg.Headers.Add("Authorization", $"Bearer {token}");

      var response = await client.SendAsync(msg);

      response.EnsureSuccessStatusCode();
      var responseBody = await Json.DeserializeResponse<List<LockDto>>(response);
      responseBody.Should().ContainSingle();
      responseBody.Should().ContainEquivalentOf(setup._lock.ToLockDto());
    }

    [Fact]
    public async Task GetById_WhenUserHasAccess_ReturnsLock() {
      var client = factory.CreateClient();
      var token = new JwtIssuer(Config.GetInstance()).WriteToken(setup.user);
      var msg = new HttpRequestMessage(HttpMethod.Get, $"api/locks/{setup._lock.Id}");
      msg.Headers.Add("Authorization", $"Bearer {token}");

      var response = await client.SendAsync(msg);

      response.EnsureSuccessStatusCode();
      var responseBody = await Json.DeserializeResponse<LockDto>(response);
      responseBody.Should().BeEquivalentTo(setup._lock.ToLockDto());
    }

    [Fact]
    public async Task GetById_WhenUserDoesNotHaveAccess_Returns403() {
      var client = factory.CreateClient();
      var token = new JwtIssuer(Config.GetInstance()).WriteToken(setup.user);
      var msg = new HttpRequestMessage(HttpMethod.Get, $"api/locks/{Int32.MaxValue}");
      msg.Headers.Add("Authorization", $"Bearer {token}");

      var response = await client.SendAsync(msg);

      response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Open_WhenUserHasAccess_ReturnsUserAccessLogDto() {
      var client = factory.CreateClient();
      var token = new JwtIssuer(Config.GetInstance()).WriteToken(setup.user);
      var msg = new HttpRequestMessage(HttpMethod.Get, $"api/locks/{setup._lock.Id}/open");
      msg.Headers.Add("Authorization", $"Bearer {token}");

      var response = await client.SendAsync(msg);

      response.EnsureSuccessStatusCode();
      var responseBody = await Json.DeserializeResponse<UserAccessLogDto>(response);
      responseBody.Id.Should().BeGreaterThan(0);
      responseBody.LockId.Should().Be(setup._lock.Id);
      responseBody.CreatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, 5.Seconds());
    }

    [Fact]
    public async Task Open_WhenUserDoesNotHaveAccess_Returns403() {
      var client = factory.CreateClient();
      var token = new JwtIssuer(Config.GetInstance()).WriteToken(setup.user);
      var msg = new HttpRequestMessage(HttpMethod.Get, $"api/locks/{Int32.MaxValue}/open");
      msg.Headers.Add("Authorization", $"Bearer {token}");

      var response = await client.SendAsync(msg);

      response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
  }
}