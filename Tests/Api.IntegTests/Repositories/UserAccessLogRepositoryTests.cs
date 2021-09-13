using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.IntegTests.Tools;
using Api.Models.Dtos;
using Domain.Models;
using FluentAssertions;
using FluentAssertions.Extensions;
using Infrastructure.Libs.Database;
using Infrastructure.Repositories;
using LinqToDB;
using Xunit;

namespace Api.IntegTests.Repositories {
  public class UserAccessLogRepositoryTests {
    public class GetUserAccessLogsSetupFixture : IDisposable {
      public readonly MainDataConnection Db = Database.GetConnection();

      public readonly User User = new() {
        Username = "User Access Log Repository Test User 1",
        Password = new LoginRequestDto { Password = "12345" }.Password
      };

      public readonly Lock Lock = new() { Name = "User Access Log Repository Test Lock 1" };
      public readonly Tag Tag = new() { Name = "User Access Log Repository Test Tag 1" };
      public readonly List<UserAccessLog> Logs = new();

      public GetUserAccessLogsSetupFixture() {
        var logsIds = (from logs in Db.UsersAccessLogs
          join users in Db.Users on logs.UserId equals users.Id
          where users.Username == User.Username
          select logs.Id).ToHashSet();
        Db.UsersAccessLogs.Where(it => logsIds.Contains(it.Id)).Delete();
        Db.Users.Where(it => it.Username == User.Username).Delete();
        Db.Tags.Where(it => it.Name == Tag.Name).Delete();
        Db.Locks.Where(it => it.Name == Lock.Name).Delete();

        Lock.Id = Db.InsertWithInt32Identity(Lock);
        User.TagId = Tag.Id = Db.InsertWithInt32Identity(Tag);
        User.Id = Db.InsertWithInt32Identity(User);
        Db.Insert(new Access { LockId = Lock.Id, TagId = Tag.Id });
        for (var i = 0; i < 3; i++) {
          var log = new UserAccessLog {
            LockId = Lock.Id,
            UserId = User.Id
          };
          log.Id = Db.InsertWithInt32Identity(log);
          Logs.Add(log);
        }
      }

      public void Dispose() {
        Db.UsersAccessLogs.Where(it => it.UserId == User.Id).Delete();
        Db.Delete(User);
        Db.Delete(Tag);
        Db.Delete(Lock);
        Db.Close();
      }
    }

    [Collection(nameof(UserAccessLogRepository))]
    public class GetUserAccessLogsTests : IClassFixture<GetUserAccessLogsSetupFixture> {
      private readonly GetUserAccessLogsSetupFixture setup;
      private readonly UserAccessLogRepository repo;

      public GetUserAccessLogsTests(GetUserAccessLogsSetupFixture setup) {
        this.setup = setup;
        this.repo = new(setup.Db);
      }

      [Fact]
      public async Task GetUserAccessLogs_WithoutFilters_ReturnsAllUserLogs() {
        var logs = await repo.GetUserAccessLogs(setup.User.Id, 1);

        logs.Should().BeEquivalentTo(
          setup.Logs, options =>
            options.Using<DateTimeOffset>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, 1.Seconds()))
              .WhenTypeIs<DateTimeOffset>()
        );
      }

      [Fact]
      public async Task GetUserAccessLogs_Without_ReturnsAllUserLogs() {
        var from = DateTimeOffset.UtcNow.Subtract(1.Minutes());
        var to = DateTimeOffset.UtcNow.Add(1.Minutes());
        var logs = await repo.GetUserAccessLogs(setup.User.Id, 1, from, to, setup.Lock.Id);

        logs.Should().BeEquivalentTo(
          setup.Logs, options =>
            options.Using<DateTimeOffset>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, 1.Seconds()))
              .WhenTypeIs<DateTimeOffset>()
        );
      }
    }

    public class LogUserAccessSetupFixture {
      public readonly MainDataConnection Db = Database.GetConnection();

      public readonly User User = new() {
        Username = "User Access Log Repository Test User 1",
        Password = new LoginRequestDto { Password = "12345" }.Password
      };

      public readonly Lock Lock = new() { Name = "User Access Log Repository Test Lock 1" };

      public LogUserAccessSetupFixture() {
        var logsIds = (from logs in Db.UsersAccessLogs
          join users in Db.Users on logs.UserId equals users.Id
          where users.Username == User.Username
          select logs.Id).ToHashSet();
        Db.UsersAccessLogs.Where(it => logsIds.Contains(it.Id)).Delete();
        Db.Users.Where(it => it.Username == User.Username).Delete();
        Db.Locks.Where(it => it.Name == Lock.Name).Delete();

        Lock.Id = Db.InsertWithInt32Identity(Lock);
        User.Id = Db.InsertWithInt32Identity(User);
      }

      public void Dispose() {
        Db.UsersAccessLogs.Where(it => it.UserId == User.Id).Delete();
        Db.Delete(User);
        Db.Delete(Lock);
        Db.Close();
      }
    }

    [Collection(nameof(UserAccessLogRepository))]
    public class LogUserAccess : IClassFixture<LogUserAccessSetupFixture> {
      private readonly UserAccessLogRepository repo;
      private readonly LogUserAccessSetupFixture setup;

      public LogUserAccess(LogUserAccessSetupFixture setup) {
        this.setup = setup;
        this.repo = new UserAccessLogRepository(setup.Db);
      }

      [Fact]
      public async Task LogsUserAccessToDb() {
        var log = await repo.LogUserAccess(setup.User.Id, setup.Lock.Id);

        log.Should().BeEquivalentTo(
          new UserAccessLog { LockId = setup.Lock.Id, UserId = setup.User.Id },
          options => options.Excluding(o => o.Id)
            .Using<DateTimeOffset>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, 1.Seconds()))
            .WhenTypeIs<DateTimeOffset>()
        );
      }
    }
  }
}