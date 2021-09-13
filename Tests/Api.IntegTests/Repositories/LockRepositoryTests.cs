using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.IntegTests.Tools;
using Api.Models.Dtos;
using Domain.Models;
using FluentAssertions;
using Infrastructure.Libs.Database;
using Infrastructure.Repositories;
using LinqToDB;
using Xunit;

namespace Api.IntegTests.Repositories {
  public class LockRepositorySetupFixture : IDisposable {
    public readonly MainDataConnection Db = Database.GetConnection();

    public readonly User User = new() {
      Username = "Lock Repository Test User 1",
      Password = new LoginRequestDto { Password = "12345" }.Password
    };

    public readonly Tag Tag = new() { Name = "Lock Repository Test Tag 1" };
    public readonly Lock Lock = new() { Name = "Lock Repository Test Lock 1" };

    public LockRepositorySetupFixture() {
      var logsIds = (from logs in Db.UsersAccessLogs
        join users in Db.Users on logs.UserId equals users.Id
        where users.Username == User.Username
        select logs.Id).ToHashSet();
      Db.UsersAccessLogs.Where(it => logsIds.Contains(it.Id)).Delete();
      Db.Users.Where(it => it.Username == User.Username).Delete();
      Db.Tags.Where(it => it.Name == Tag.Name).Delete();
      Db.Locks.Where(it => it.Name == Lock.Name).Delete();

      User.TagId = Tag.Id = Db.InsertWithInt32Identity(Tag);
      Lock.Id = Db.InsertWithInt32Identity(Lock);
      User.Id = Db.InsertWithInt32Identity(User);
      Db.Insert(new Access { LockId = Lock.Id, TagId = Tag.Id });
    }

    public void Dispose() {
      Db.UsersAccessLogs.Where(it => it.UserId == User.Id).Delete();
      Db.Delete(User);
      Db.Delete(Tag);
      Db.Delete(Lock);
      Db.Close();
    }
  }

  public class LockRepositoryTests : IClassFixture<LockRepositorySetupFixture> {
    private readonly LockRepositorySetupFixture setup;
    private readonly LockRepository repo;

    public LockRepositoryTests(LockRepositorySetupFixture setup) {
      this.setup = setup;
      this.repo = new(setup.Db);
    }

    [Fact]
    public async Task GetAccessibleByUser_WhenUserHasAccess_ReturnsListOfLocks() {
      var locks = await repo.GetAccessibleByUser(setup.User.Id);

      locks.Should().BeEquivalentTo(new List<Lock>() { setup.Lock });
    }

    [Fact]
    public async Task GetAccessibleByUser_WhenUserHasAccess_ReturnsLock() {
      var user = await repo.GetAccessibleByUser(setup.User.Id, setup.Lock.Id);

      user.Should().BeEquivalentTo(setup.Lock);
    }

    [Fact]
    public async Task GetAccessibleByUser_WhenUserDoesNotHaveAccess_ReturnsNull() {
      var user = await repo.GetAccessibleByUser(setup.User.Id, int.MaxValue);

      user.Should().BeNull();
    }
  }
}