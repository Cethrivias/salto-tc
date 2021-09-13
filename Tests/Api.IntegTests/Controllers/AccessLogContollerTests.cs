using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Api.IntegTests.Tools;
using Api.Models.Dtos;
using Api.Utils;
using Api.Utils.Extensions;
using Domain.Models;
using FluentAssertions;
using FluentAssertions.Extensions;
using Infrastructure.Libs.Database;
using LinqToDB;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Xunit.Abstractions;

namespace Api.IntegTests.Controllers {
  public class AccessLogControllerSetupFixture : IDisposable {
    private readonly MainDataConnection db = Database.GetConnection();

    public readonly User User = new() {
      Username = "Access Log Test User 1",
      Password = new LoginRequestDto { Password = "12345" }.Password
    };

    public readonly Tag Tag = new() { Name = "Access Log Test Tag 1" };
    public readonly Lock Lock = new() { Name = "Access Log Test Lock 1" };
    public readonly List<UserAccessLog> Logs = new();

    public AccessLogControllerSetupFixture() {
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
      for (var i = 0; i < 3; i++) {
        var log = new UserAccessLog { UserId = User.Id, LockId = Lock.Id };
        log.Id = db.InsertWithInt32Identity(log);
        Logs.Add(log);
      }
    }

    public void Dispose() {
      db.UsersAccessLogs.Where(it => it.UserId == User.Id).Delete();
      db.Delete(User);
      db.Delete(Tag);
      db.Delete(Lock);
      db.Close();
    }
  }

  public class AccessLogControllerTests : IClassFixture<WebApplicationFactory<Startup>>,
    IClassFixture<AccessLogControllerSetupFixture> {
    private readonly WebApplicationFactory<Startup> factory;
    private readonly AccessLogControllerSetupFixture setup;
    private readonly ITestOutputHelper testOutputHelper;

    public AccessLogControllerTests(
      WebApplicationFactory<Startup> factory,
      AccessLogControllerSetupFixture setup, ITestOutputHelper testOutputHelper
    ) {
      this.factory = factory;
      this.setup = setup;
      this.testOutputHelper = testOutputHelper;
    }

    [Fact]
    public async Task Get_ReturnsUserLogs() {
      var client = factory.CreateClient();
      var token = new JwtIssuer(Config.GetInstance()).WriteToken(setup.User);
      var msg = new HttpRequestMessage(HttpMethod.Get, "api/access-logs");
      msg.Headers.Add("Authorization", $"Bearer {token}");

      var response = await client.SendAsync(msg);

      response.EnsureSuccessStatusCode();
      var responseBody = await Json.DeserializeResponse<PaginatedAccessLogsDto>(response);
      var expected = new PaginatedAccessLogsDto {
        Data = setup.Logs.ConvertAll(item => item.ToUserAccessLogDto()),
        Page = 1,
        Pages = 1
      };
      var db = Database.GetConnection();
      var users1 = (from users in db.Users
        select users).ToList();
      var tags1 = (from tags in db.Tags
        select tags).ToList();
      var access1 = (from access in db.Access
        select access).ToList();
      var locks1 = (from locks in db.Locks
        select locks).ToList();
      var userAccessLogs1 = (from userAccessLogs in db.UsersAccessLogs
        select userAccessLogs).ToList();
      testOutputHelper.WriteLine($"Users: {Json.Serialize(users1)}");
      testOutputHelper.WriteLine($"Tags: {Json.Serialize(tags1)}");
      testOutputHelper.WriteLine($"Access: {Json.Serialize(access1)}");
      testOutputHelper.WriteLine($"Locks: {Json.Serialize(locks1)}");
      testOutputHelper.WriteLine($"Logs: {Json.Serialize(userAccessLogs1)}");

      responseBody.Should().BeEquivalentTo(
        expected, options =>
          options
            .Using<DateTimeOffset>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, 5.Seconds()))
            .WhenTypeIs<DateTimeOffset>()
      );
    }
  }
}