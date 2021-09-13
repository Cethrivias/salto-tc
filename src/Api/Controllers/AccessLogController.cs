using System;
using System.Linq;
using System.Threading.Tasks;
using Api.Models.Dtos;
using Api.Utils;
using Api.Utils.Extensions;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers {

  [ApiController]
  [Route("api/access-logs")]
  [Produces("application/json")]
  [Authorize]
  public class AccessLogController : ControllerBase {
    private readonly IUserAccessLogService userAccessLogService;
    private readonly IUserProvider userProvider;

    public AccessLogController(IUserAccessLogService userAccessLogService, IUserProvider userProvider) {
      this.userAccessLogService = userAccessLogService;
      this.userProvider = userProvider;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedAccessLogsDto>> Get(
      [FromQuery] int page = 1,
      [FromQuery(Name = "from")] DateTimeOffset? createdAtFrom = null,
      [FromQuery(Name = "to")] DateTimeOffset? createdAtTo = null,
      [FromQuery] int? lockId = null
    ) {
      var userId = userProvider.UserId;

      var logs = await userAccessLogService.GetUserAccessLogs(userId, page, createdAtFrom, createdAtTo, lockId);
      var pages = await userAccessLogService.GetUserAccessLogsPages(userId, createdAtFrom, createdAtTo, lockId);

      var response = new PaginatedAccessLogsDto {
        Data = logs.Select(it => it.ToUserAccessLogDto()).ToList(),
        Page = page,
        Pages = pages
      };

      return Ok(response);
    }
  }
}