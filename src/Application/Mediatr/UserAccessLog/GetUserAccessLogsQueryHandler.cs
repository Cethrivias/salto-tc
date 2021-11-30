using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces.Repositories;
using Application.Models;
using MediatR;

namespace Application.Mediatr.UserAccessLog
{
  public class
    GetUserAccessLogsQueryHandler : IRequestHandler<GetUserAccessLogsQuery,
      PaginatedResponse<Domain.Models.UserAccessLog>>
  {
    private readonly IUserAccessLogRepository _userAccessLogRepository;

    public GetUserAccessLogsQueryHandler(IUserAccessLogRepository userAccessLogRepository)
    {
      _userAccessLogRepository = userAccessLogRepository;
    }

    public async Task<PaginatedResponse<Domain.Models.UserAccessLog>> Handle(
      GetUserAccessLogsQuery request, CancellationToken cancellationToken
    )
    {
      var logs = await _userAccessLogRepository.GetUserAccessLogs(
        request.UserId,
        request.Page,
        request.CreatedAtFrom,
        request.CreatedAtTo,
        request.LockId
      );
      var pages = await _userAccessLogRepository.GetUserAccessLogsPages(
        request.UserId,
        request.CreatedAtFrom,
        request.CreatedAtTo,
        request.LockId
      );

      return new PaginatedResponse<Domain.Models.UserAccessLog>
      {
        Data = logs,
        Page = request.Page,
        Pages = pages,
      };
    }
  }
}