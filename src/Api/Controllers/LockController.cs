using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Models.Dtos;
using Api.Utils;
using Api.Utils.Extensions;
using Application.Mediatr.Lock;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers {

  [ApiController]
  [Route("api/locks")]
  [Produces("application/json")]
  [Authorize]
  public class LockController : ControllerBase {
    private readonly IUserProvider _userProvider;

    private readonly IMediator _mediator;

    public LockController(
      IUserProvider userProvider,
      IMediator mediator
    ) {
      _userProvider = userProvider;
      _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<LockDto>>> Get() {
      var userId = _userProvider.UserId;
      var query = new GetLocksQuery
      {
        UserId = userId
      };

      var locks = await _mediator.Send(query);

      return Ok(locks.Select(it => it.ToLockDto()).ToList());
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<LockDto>> GetById([FromRoute(Name = "id")] int lockId) {
      var userId = _userProvider.UserId;

      var query = new GetLockQuery
      {
        LockId = lockId,
        UserId = userId
      };

      var item = await _mediator.Send(query);
      
      if (item is null) {
        return Forbid();
      }

      return Ok(item.ToLockDto());
    }

    [HttpGet("{id}/open")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<UserAccessLogDto>> Open([FromRoute(Name = "id")] int lockId) {
      var userId = _userProvider.UserId;

      var command = new OpenLockCommand
      {
        LockId = lockId,
        UserId = userId
      };

      var userAccessLog = await _mediator.Send(command);

      if (userAccessLog is null) {
        return Forbid();
      }

      return Ok(userAccessLog.ToUserAccessLogDto());
    }
  }
}