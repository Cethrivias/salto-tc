using System;
using System.Linq;
using System.Threading.Tasks;
using Api.Models.Dtos;
using Api.Utils;
using Api.Utils.Extensions;
using Application.Mediatr.UserAccessLog;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
  [ApiController]
  [Route("api/access-logs")]
  [Produces("application/json")]
  [Authorize]
  public class AccessLogController : ControllerBase
  {
    private readonly IUserProvider _userProvider;
    private readonly IMediator _mediator;

    public AccessLogController(IUserProvider userProvider, IMediator mediator)
    {
      _userProvider = userProvider;
      _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedAccessLogsDto>> Get(
      [FromQuery] int page = 1,
      [FromQuery(Name = "from")] DateTimeOffset? createdAtFrom = null,
      [FromQuery(Name = "to")] DateTimeOffset? createdAtTo = null,
      [FromQuery] int? lockId = null
    )
    {
      var userId = _userProvider.UserId;

      var query = new GetUserAccessLogsQuery
      {
        Page = page,
        LockId = lockId,
        UserId = userId,
        CreatedAtFrom = createdAtFrom,
        CreatedAtTo = createdAtTo
      };

      var paginatedUserAccessLog = await _mediator.Send(query);

      var response = new PaginatedAccessLogsDto
      {
        Data = paginatedUserAccessLog.Data.Select(it => it.ToUserAccessLogDto()).ToList(),
        Page = paginatedUserAccessLog.Page,
        Pages = paginatedUserAccessLog.Pages
      };

      return Ok(response);
    }
  }
}