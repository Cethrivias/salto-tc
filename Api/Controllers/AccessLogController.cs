using System;
using System.Linq;
using System.Threading.Tasks;
using Api.Models.Dtos;
using Api.Repositories;
using Api.Utils;
using Api.Utils.Extentions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers {

  [ApiController]
  [Route("api/access-logs")]
  [Produces("application/json")]
  [Authorize]
  public class AccessLogController : ControllerBase {
    private readonly IUserAccessLogRepository userAccessLogRepository;
    private readonly IUserProvider userProvider;

    public AccessLogController(IUserAccessLogRepository userAccessLogRepository, IUserProvider userProvider) {
      this.userAccessLogRepository = userAccessLogRepository;
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

      var logs = await userAccessLogRepository.GetUserAccessLogs(userId, page, createdAtFrom, createdAtTo, lockId);
      var pages = await userAccessLogRepository.GetUserAccessLogsPages(userId, createdAtFrom, createdAtTo, lockId);

      var response = new PaginatedAccessLogsDto {
        data = logs.Select(it => it.ToUserAccessLogDto()).ToList(),
        page = page,
        pages = pages
      };

      return Ok(response);
    }
  }
}