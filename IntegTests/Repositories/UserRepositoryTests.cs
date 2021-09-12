using Api.Libs.Database;
using Api.Models;
using Api.Models.Dtos;
using Api.Repositories;
using FluentAssertions;
using IntegTests.Tools;
using LinqToDB;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace IntegTests.Repositories {
  public class UserRepositorySetupFixture : IDisposable {
    public readonly MainDataConnection Db = Database.GetConnection();

    public readonly User User = new() {
      Username = "User Repository Test User 1",
      Password = new LoginRequestDto { Password = "12345" }.Password
    };

    public readonly Tag Tag = new() { Name = "User Repository Test Tag 1" };
    public readonly Lock Lock = new() { Name = "User Repository Test Lock 1" };

    public UserRepositorySetupFixture() {
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

  public class UserRepositoryTests : IClassFixture<UserRepositorySetupFixture> {
    private readonly UserRepositorySetupFixture setup;
    private readonly UserRepository repo;

    public UserRepositoryTests(UserRepositorySetupFixture setup) {
      this.setup = setup;
      this.repo = new(setup.Db);
    }

    [Fact]
    public async Task GetByCredentials_WhenUserExists_ReturnsUser() {
      var creds = new LoginRequestDto {
        Password = "12345",
        Username = "User Repository Test User 1"
      };
      var user = await repo.GetByCredentials(creds);

      user.Should().BeEquivalentTo(creds, options => options.ExcludingMissingMembers());
    }

    [Fact]
    public async Task GetByCredentials_WhenUsingWrongPassword_ReturnsNull() {
      var creds = new LoginRequestDto {
        Password = "123456",
        Username = "User Repository Test User 1"
      };
      var user = await repo.GetByCredentials(creds);

      user.Should().BeNull();
    }

    [Fact]
    public async Task GetByCredentials_WhenUserDoesNotExists_ReturnsNull() {
      var creds = new LoginRequestDto {
        Password = "12345",
        Username = "User Repository Test User 2"
      };
      var user = await repo.GetByCredentials(creds);

      user.Should().BeNull();
    }

    [Fact]
    public async Task HasAccess_WhenUserHasAccess_ReturnsTrue() {
      var _lock = await repo.HasAccess(setup.User.Id, setup.Lock.Id);
      _lock.Should().BeTrue();
    }

    [Fact]
    public async Task HasAccess_WhenUserDoesNotHaveAccess_ReturnsFalse() {
      var _lock = await repo.HasAccess(setup.User.Id, int.MaxValue);
      _lock.Should().BeFalse();
    }
  }
}