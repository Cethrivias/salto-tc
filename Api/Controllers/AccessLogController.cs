using System;
using System.Linq;
using System.Threading.Tasks;
using Api.Models.Dtos;
using Api.Repositories;
using Api.Utils.Extentions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers {

  [ApiController]
  [Route("api/access-logs")]
  [Produces("application/json")]
  [Authorize]
  public class AccessLogController : ControllerBase {
    private readonly IUserAccessLogRepository userAccessLogRepository;

    public AccessLogController(IUserAccessLogRepository userAccessLogRepository) {
      this.userAccessLogRepository = userAccessLogRepository;
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedAccessLogsDto>> Get(
      [FromQuery] int page = 1,
      [FromQuery(Name = "from")] DateTimeOffset? createdAtFrom = null,
      [FromQuery(Name = "to")] DateTimeOffset? createdAtTo = null,
      [FromQuery] int? lockId = null
    ) {
      var userId = HttpContext.GetUserId();

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