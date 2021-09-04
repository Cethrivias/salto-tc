using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Api.Models;
using Api.Models.Dtos;
using Api.Repositories;
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
    public async Task<ActionResult<PaginatedAccessLogResponseDto>> Get(
      [FromQuery] int page = 1,
      [FromQuery(Name = "from")] DateTimeOffset? createdAtFrom = null,
      [FromQuery(Name = "to")] DateTimeOffset? createdAtTo = null,
      [FromQuery] int? lockId = null
    ) {
      var nameIdentifier = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
      var userId = Int32.Parse(nameIdentifier);

      var logs = await userAccessLogRepository.GetUserAccessLogs(userId, page, createdAtFrom, createdAtTo, lockId);
      var pages = await userAccessLogRepository.GetUserAccessLogsPages(userId, createdAtFrom, createdAtTo, lockId);

      var response = new PaginatedAccessLogResponseDto {
        data = logs,
        page = page,
        pages = pages
      };

      return Ok(response);
    }
  }
}