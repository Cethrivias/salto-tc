using System.Collections.Generic;
using Api.Controllers;
using Api.Models;
using Api.Models.Dtos;
using Api.Repositories;
using Api.Utils;
using Api.Utils.Extentions;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Tests.Extensions;
using Xunit;

namespace Tests.Controllers {
  public class LockControllerTests {
    [Fact]
    public async void Get_ReturnsListOfLocks() {
      var lockRepoMock = new Mock<ILockRepository>();
      var userRepoMock = new Mock<IUserRepository>();
      var userAccessLogRepoMock = new Mock<IUserAccessLogRepository>();
      var userProviderMock = new Mock<IUserProvider>();

      var _lock = new Lock {
        Id = 1,
        Name = "Tunnel"
      };

      lockRepoMock.Setup(repo => repo.GetAccessibleByUser(It.IsAny<int>())).ReturnsAsync(new List<Lock> { _lock });
      userProviderMock.Setup(provider => provider.UserId).Returns(1);

      var lockController = new LockController(
        lockRepoMock.Object,
        userRepoMock.Object,
        userAccessLogRepoMock.Object,
        userProviderMock.Object
      );

      var response = await lockController.Get();
      var value = response.GetObjectResult();

      value.Should().BeEquivalentTo(new List<LockDto> { _lock.ToLockDto() });
    }

    [Fact]
    public async void GetById_WhenUserHasAccess_ReturnsLock() {
      var lockRepoMock = new Mock<ILockRepository>();
      var userRepoMock = new Mock<IUserRepository>();
      var userAccessLogRepoMock = new Mock<IUserAccessLogRepository>();
      var userProviderMock = new Mock<IUserProvider>();

      var _lock = new Lock {
        Id = 1,
        Name = "Tunnel"
      };

      lockRepoMock.Setup(
        repo => repo.GetAccessibleByUser(It.IsAny<int>(), It.Is<int>(lockId => lockId == _lock.Id))
      ).ReturnsAsync(_lock);
      userProviderMock.Setup(provider => provider.UserId).Returns(1);

      var lockController = new LockController(
        lockRepoMock.Object,
        userRepoMock.Object,
        userAccessLogRepoMock.Object,
        userProviderMock.Object
      );

      var response = await lockController.GetById(_lock.Id);
      var value = response.GetObjectResult();

      value.Should().BeEquivalentTo(_lock.ToLockDto());
    }

    [Fact]
    public async void GetById_WhenUserDoesNotHaveAccess_Returns403() {
      var lockRepoMock = new Mock<ILockRepository>();
      var userRepoMock = new Mock<IUserRepository>();
      var userAccessLogRepoMock = new Mock<IUserAccessLogRepository>();
      var userProviderMock = new Mock<IUserProvider>();

      lockRepoMock.Setup(
        repo => repo.GetAccessibleByUser(It.IsAny<int>(), It.IsAny<int>())
      ).ReturnsAsync((Lock)null);
      userProviderMock.Setup(provider => provider.UserId).Returns(1);

      var lockController = new LockController(
        lockRepoMock.Object,
        userRepoMock.Object,
        userAccessLogRepoMock.Object,
        userProviderMock.Object
      );

      var response = await lockController.GetById(1);

      response.Result.Should().BeOfType<ForbidResult>();
    }

    [Fact]
    public async void Open_WhenUserHasAccess_OpensLock() {
      var lockRepoMock = new Mock<ILockRepository>();
      var userRepoMock = new Mock<IUserRepository>();
      var userAccessLogRepoMock = new Mock<IUserAccessLogRepository>();
      var userProviderMock = new Mock<IUserProvider>();

      var userAccessLog = new UserAccessLog {
        LockId = 1,
        UserId = 2,
        Id = 3,
      };

      userRepoMock.Setup(
        repo => repo.HasAccess(
          It.Is<int>(it => it == userAccessLog.UserId),
          It.Is<int>(it => it == userAccessLog.LockId)
        )
      ).ReturnsAsync(true);
      userAccessLogRepoMock.Setup(repo => repo.LogUserAccess(
        It.Is<int>(it => it == userAccessLog.UserId),
        It.Is<int>(it => it == userAccessLog.LockId)
      )).ReturnsAsync(userAccessLog);
      userProviderMock.Setup(provider => provider.UserId).Returns(userAccessLog.UserId);

      var lockController = new LockController(
        lockRepoMock.Object,
        userRepoMock.Object,
        userAccessLogRepoMock.Object,
        userProviderMock.Object
      );

      var response = await lockController.Open(userAccessLog.LockId);

      response.Result.Should().BeOfType<OkObjectResult>();
      var value = response.GetObjectResult();
      value.Should().BeEquivalentTo(userAccessLog.ToUserAccessLogDto());
    }

    [Fact]
    public async void Open_WhenUserDoesNotHaveAccess_Returns403() {
      var lockRepoMock = new Mock<ILockRepository>();
      var userRepoMock = new Mock<IUserRepository>();
      var userAccessLogRepoMock = new Mock<IUserAccessLogRepository>();
      var userProviderMock = new Mock<IUserProvider>();

      var userAccessLog = new UserAccessLog {
        LockId = 1,
        UserId = 2,
        Id = 3,
      };

      userRepoMock.Setup(
        repo => repo.HasAccess(
          It.Is<int>(it => it == userAccessLog.UserId),
          It.Is<int>(it => it == userAccessLog.LockId)
        )
      ).ReturnsAsync(false);
      userProviderMock.Setup(provider => provider.UserId).Returns(userAccessLog.UserId);

      var lockController = new LockController(
        lockRepoMock.Object,
        userRepoMock.Object,
        userAccessLogRepoMock.Object,
        userProviderMock.Object
      );

      var response = await lockController.Open(userAccessLog.LockId);

      userAccessLogRepoMock.Verify(
        repo => repo.LogUserAccess(It.IsAny<int>(), It.IsAny<int>()),
        Times.Never
      );
      response.Result.Should().BeOfType<ForbidResult>();
    }
  }
}