using System.Linq;
using System.Threading;
using Api.Controllers;
using Api.Models.Dtos;
using Api.Utils;
using Api.Utils.Extensions;
using Application.Mediatr.UserAccessLog;
using Application.Models;
using Domain.Models;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Api.UnitTests.Controllers
{
  public class AccessLogControllerTests
  {
    private readonly Mock<IMediator> _mediator;
    private readonly Mock<IUserProvider> _userProvider;
    private readonly AccessLogController _sut;

    public AccessLogControllerTests()
    {
      _mediator = new Mock<IMediator>();
      _userProvider = new Mock<IUserProvider>();
      _sut = new AccessLogController(_userProvider.Object, _mediator.Object);
    }

    [Fact]
    public async void Get_ReturnsPaginatedAccessLogs()
    {
      var paginatedResponse = new PaginatedResponse<UserAccessLog>
      {
        Data = new[] { new UserAccessLog() },
        Page = 1,
        Pages = 1
      };
      _mediator.Setup(x => x.Send(It.IsAny<GetUserAccessLogsQuery>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync(paginatedResponse);
      _userProvider.Setup(provider => provider.UserId).Returns(1);

      var response = await _sut.Get();

      response.Result.Should().BeOfType<OkObjectResult>();
      var value = response.GetObjectResult();
      var expected = new PaginatedAccessLogsDto
      {
        Data = paginatedResponse.Data.Select(it => it.ToUserAccessLogDto()).ToList(),
        Page = paginatedResponse.Page,
        Pages = paginatedResponse.Pages
      };
      value.Should().BeEquivalentTo(expected);
    }
  }
}