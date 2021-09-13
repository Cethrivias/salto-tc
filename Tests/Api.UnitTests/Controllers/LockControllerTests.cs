using System.Collections.Generic;
using Api.Controllers;
using Api.Models.Dtos;
using Api.Utils;
using Api.Utils.Extensions;
using Application.Core;
using Application.Services;
using Domain.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Api.UnitTests.Controllers {
  public class LockControllerTests {
    [Fact]
    public async void Get_ReturnsListOfLocks() {
      var lockOpenerMock = new Mock<ILockOpener>();
      var lockServiceMock = new Mock<ILockService>();
      var userServiceMock = new Mock<IUserService>();
      var userAccessLogServiceMock = new Mock<IUserAccessLogService>();
      var userProviderMock = new Mock<IUserProvider>();

      var _lock = new Lock {
        Id = 1,
        Name = "Tunnel"
      };

      lockServiceMock.Setup(repo => repo.GetAccessibleByUser(It.IsAny<int>())).ReturnsAsync(new List<Lock> { _lock });
      userProviderMock.Setup(provider => provider.UserId).Returns(1);

      var lockController = new LockController(
        lockOpenerMock.Object,
        lockServiceMock.Object,
        userServiceMock.Object,
        userAccessLogServiceMock.Object,
        userProviderMock.Object
      );

      var response = await lockController.Get();
      var value = response.GetObjectResult();

      value.Should().BeEquivalentTo(new List<LockDto> { _lock.ToLockDto() });
    }

    [Fact]
    public async void GetById_WhenUserHasAccess_ReturnsLock() {
      var lockOpenerMock = new Mock<ILockOpener>();
      var lockServiceMock = new Mock<ILockService>();
      var userServiceMock = new Mock<IUserService>();
      var userAccessLogServiceMock = new Mock<IUserAccessLogService>();
      var userProviderMock = new Mock<IUserProvider>();

      var _lock = new Lock {
        Id = 1,
        Name = "Tunnel"
      };

      lockServiceMock.Setup(
        repo => repo.GetAccessibleByUser(It.IsAny<int>(), It.Is<int>(lockId => lockId == _lock.Id))
      ).ReturnsAsync(_lock);
      userProviderMock.Setup(provider => provider.UserId).Returns(1);

      var lockController = new LockController(
        lockOpenerMock.Object,
        lockServiceMock.Object,
        userServiceMock.Object,
        userAccessLogServiceMock.Object,
        userProviderMock.Object
      );

      var response = await lockController.GetById(_lock.Id);
      var value = response.GetObjectResult();

      value.Should().BeEquivalentTo(_lock.ToLockDto());
    }

    [Fact]
    public async void GetById_WhenUserDoesNotHaveAccess_Returns403() {
      var lockOpenerMock = new Mock<ILockOpener>();
      var lockServiceMock = new Mock<ILockService>();
      var userServiceMock = new Mock<IUserService>();
      var userAccessLogServiceMock = new Mock<IUserAccessLogService>();
      var userProviderMock = new Mock<IUserProvider>();

      lockServiceMock.Setup(
        repo => repo.GetAccessibleByUser(It.IsAny<int>(), It.IsAny<int>())
      ).ReturnsAsync((Lock)null);
      userProviderMock.Setup(provider => provider.UserId).Returns(1);

      var lockController = new LockController(
        lockOpenerMock.Object,
        lockServiceMock.Object,
        userServiceMock.Object,
        userAccessLogServiceMock.Object,
        userProviderMock.Object
      );

      var response = await lockController.GetById(1);

      response.Result.Should().BeOfType<ForbidResult>();
    }

    [Fact]
    public async void Open_WhenUserHasAccess_OpensLock() {
      var lockOpenerMock = new Mock<ILockOpener>();
      var lockServiceMock = new Mock<ILockService>();
      var userServiceMock = new Mock<IUserService>();
      var userAccessLogServiceMock = new Mock<IUserAccessLogService>();
      var userProviderMock = new Mock<IUserProvider>();

      var userAccessLog = new UserAccessLog {
        LockId = 1,
        UserId = 2,
        Id = 3,
      };

      userServiceMock.Setup(
        repo => repo.HasAccess(
          It.Is<int>(it => it == userAccessLog.UserId),
          It.Is<int>(it => it == userAccessLog.LockId)
        )
      ).ReturnsAsync(true);
      userAccessLogServiceMock.Setup(repo => repo.LogUserAccess(
        It.Is<int>(it => it == userAccessLog.UserId),
        It.Is<int>(it => it == userAccessLog.LockId)
      )).ReturnsAsync(userAccessLog);
      userProviderMock.Setup(provider => provider.UserId).Returns(userAccessLog.UserId);

      var lockController = new LockController(
        lockOpenerMock.Object,
        lockServiceMock.Object,
        userServiceMock.Object,
        userAccessLogServiceMock.Object,
        userProviderMock.Object
      );

      var response = await lockController.Open(userAccessLog.LockId);

      response.Result.Should().BeOfType<OkObjectResult>();
      var value = response.GetObjectResult();
      value.Should().BeEquivalentTo(userAccessLog.ToUserAccessLogDto());
    }

    [Fact]
    public async void Open_WhenUserDoesNotHaveAccess_Returns403() {
      var lockOpenerMock = new Mock<ILockOpener>();
      var lockServiceMock = new Mock<ILockService>();
      var userServiceMock = new Mock<IUserService>();
      var userAccessLogServiceMock = new Mock<IUserAccessLogService>();
      var userProviderMock = new Mock<IUserProvider>();

      var userAccessLog = new UserAccessLog {
        LockId = 1,
        UserId = 2,
        Id = 3,
      };

      userServiceMock.Setup(
        repo => repo.HasAccess(
          It.Is<int>(it => it == userAccessLog.UserId),
          It.Is<int>(it => it == userAccessLog.LockId)
        )
      ).ReturnsAsync(false);
      userProviderMock.Setup(provider => provider.UserId).Returns(userAccessLog.UserId);

      var lockController = new LockController(
        lockOpenerMock.Object,
        lockServiceMock.Object,
        userServiceMock.Object,
        userAccessLogServiceMock.Object,
        userProviderMock.Object
      );

      var response = await lockController.Open(userAccessLog.LockId);

      userAccessLogServiceMock.Verify(
        repo => repo.LogUserAccess(It.IsAny<int>(), It.IsAny<int>()),
        Times.Never
      );
      response.Result.Should().BeOfType<ForbidResult>();
    }
  }
}