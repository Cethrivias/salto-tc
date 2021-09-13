using System;
using System.Collections.Generic;
using Api.Controllers;
using Api.Models.Dtos;
using Api.Utils;
using Api.Utils.Extensions;
using Application.Services;
using Domain.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Api.UnitTests.Controllers {
  public class AccessLogControllerTests {
    [Fact]
    public async void Get_ReturnsPaginatedAccessLogs() {
      var userAccessLogServiceMock = new Mock<IUserAccessLogService>();
      var userProviderMock = new Mock<IUserProvider>();
      var userAccessLog = new UserAccessLog();
      var pages = 1;

      userAccessLogServiceMock.Setup(repo => repo.GetUserAccessLogs(
        It.IsAny<int>(),
        It.IsAny<int>(),
        It.IsAny<DateTimeOffset?>(),
        It.IsAny<DateTimeOffset?>(),
        It.IsAny<int?>()
      )).ReturnsAsync(new List<UserAccessLog> { userAccessLog });
      userAccessLogServiceMock.Setup(repo => repo.GetUserAccessLogsPages(
        It.IsAny<int>(),
        It.IsAny<DateTimeOffset?>(),
        It.IsAny<DateTimeOffset?>(),
        It.IsAny<int?>()
      )).ReturnsAsync(pages);
      userProviderMock.Setup(provider => provider.UserId).Returns(1);

      var controller = new AccessLogController(userAccessLogServiceMock.Object, userProviderMock.Object);

      var response = await controller.Get();

      response.Result.Should().BeOfType<OkObjectResult>();
      var value = response.GetObjectResult();
      var expected = new PaginatedAccessLogsDto {
        Data = new List<UserAccessLogDto> { userAccessLog.ToUserAccessLogDto() },
        Page = 1,
        Pages = pages
      };
      value.Should().BeEquivalentTo(expected);
    }
  }
}