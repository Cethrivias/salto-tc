using System.Collections.Generic;
using System.Threading;
using Api.Controllers;
using Api.Models.Dtos;
using Api.Utils;
using Api.Utils.Extensions;
using Application.Mediatr.Lock;
using Domain.Models;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Api.UnitTests.Controllers
{
  public class LockControllerTests
  {
    private readonly LockController _sut;
    private readonly Mock<IUserProvider> _userProvider;
    private readonly Mock<IMediator> _mediator;

    public LockControllerTests()
    {
      _userProvider = new Mock<IUserProvider>();
      _mediator = new Mock<IMediator>();
      _sut = new LockController(_userProvider.Object, _mediator.Object);
    }

    [Fact]
    public async void Get_ReturnsListOfLocks()
    {
      var _lock = new Lock
      {
        Id = 1,
        Name = "Tunnel"
      };

      _mediator.Setup(mediator => mediator.Send(It.IsAny<GetLocksQuery>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync(new List<Lock> { _lock });
      _userProvider.Setup(provider => provider.UserId).Returns(1);

      var response = await _sut.Get();

      var value = response.GetObjectResult();

      value.Should().BeEquivalentTo(new List<LockDto> { _lock.ToLockDto() });
    }

    [Fact]
    public async void GetById_WhenUserHasAccess_ReturnsLock()
    {
      var _lock = new Lock
      {
        Id = 1,
        Name = "Tunnel"
      };

      _mediator.Setup(mediator => mediator.Send(It.IsAny<GetLockQuery>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync(_lock);
      _userProvider.Setup(provider => provider.UserId).Returns(1);

      var response = await _sut.GetById(_lock.Id);
      var value = response.GetObjectResult();

      value.Should().BeEquivalentTo(_lock.ToLockDto());
    }

    [Fact]
    public async void GetById_WhenUserDoesNotHaveAccess_Returns403()
    {
      _mediator.Setup(x => x.Send(It.IsAny<GetLockQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync((Lock) null);
      _userProvider.Setup(provider => provider.UserId).Returns(1);

      var response = await _sut.GetById(1);

      response.Result.Should().BeOfType<ForbidResult>();
    }

    [Fact]
    public async void Open_WhenUserHasAccess_OpensLock()
    {
      var userAccessLog = new UserAccessLog
      {
        LockId = 1,
        UserId = 2,
        Id = 3,
      };
      
      _mediator.Setup(x => x.Send(It.IsAny<OpenLockCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(userAccessLog);
      _userProvider.Setup(provider => provider.UserId).Returns(userAccessLog.UserId);

      var response = await _sut.Open(userAccessLog.LockId);

      response.Result.Should().BeOfType<OkObjectResult>();
      var value = response.GetObjectResult();
      value.Should().BeEquivalentTo(userAccessLog.ToUserAccessLogDto());
    }

    [Fact]
    public async void Open_WhenUserDoesNotHaveAccess_Returns403()
    {

      var userAccessLog = new UserAccessLog
      {
        LockId = 1,
        UserId = 2,
        Id = 3,
      };

      _mediator.Setup(x => x.Send(It.IsAny<OpenLockCommand>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync((UserAccessLog) null);
      _userProvider.Setup(provider => provider.UserId).Returns(userAccessLog.UserId);

      var response = await _sut.Open(userAccessLog.LockId);
      response.Result.Should().BeOfType<ForbidResult>();
    }
  }
}