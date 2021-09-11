using System;
using System.Collections.Generic;
using System.Linq;
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
  public class AccessLogControllerSetupFixture : IDisposable {
    private readonly MainDataConnection db = Database.GetConnection();

    public User user = new User {
      Username = "Access Log Test User 1",
      Password = new LoginRequestDto { Password = "12345" }.Password
    };
    public Tag tag = new Tag { Name = "Access Log Test Tag 1" };
    public Lock _lock = new Lock { Name = "Access Log Test Lock 1" };
    public List<UserAccessLog> logs = new();

    public AccessLogControllerSetupFixture() {
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
      for (var i = 0; i < 3; i++) {
        var log = new UserAccessLog { UserId = user.Id, LockId = _lock.Id };
        log.Id = db.InsertWithInt32Identity(log);
        logs.Add(log);
      }
    }

    public void Dispose() {
      db.UsersAccessLogs.Where(it => it.UserId == user.Id).Delete();
      db.Delete(user);
      db.Delete(tag);
      db.Delete(_lock);
      db.Close();
    }
  }


  public class AccessLogControllerTests : IClassFixture<WebApplicationFactory<Api.Startup>>, IClassFixture<AccessLogControllerSetupFixture> {
    private readonly WebApplicationFactory<Startup> factory;
    private readonly AccessLogControllerSetupFixture setup;
    public AccessLogControllerTests(
      WebApplicationFactory<Api.Startup> factory,
      AccessLogControllerSetupFixture setup
    ) {
      this.factory = factory;
      this.setup = setup;
    }

    [Fact]
    public async Task Get_ReturnsUserLogs() {
      var client = factory.CreateClient();
      var token = new JwtIssuer(Config.GetInstance()).WriteToken(setup.user);
      var msg = new HttpRequestMessage(HttpMethod.Get, "api/access-logs");
      msg.Headers.Add("Authorization", $"Bearer {token}");

      var response = await client.SendAsync(msg);

      response.EnsureSuccessStatusCode();
      var responseBody = await Json.DeserializeResponse<PaginatedAccessLogsDto>(response);
      var expected = new PaginatedAccessLogsDto {
        data = setup.logs.ConvertAll(item => item.ToUserAccessLogDto()),
        page = 1,
        pages = 1
      };
      responseBody.Should().BeEquivalentTo(expected, options => 
        options 
          .Using<DateTimeOffset>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, 5.Seconds()))
          .WhenTypeIs<DateTimeOffset>()
      );
    }
  }
}