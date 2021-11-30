using System;
using Application.Models;
using MediatR;

namespace Application.Mediatr.UserAccessLog
{
  public class GetUserAccessLogsQuery : IRequest<PaginatedResponse<Domain.Models.UserAccessLog>>
  {
    public int UserId { get; init; }
    public int? LockId { get; init; }
    public DateTimeOffset? CreatedAtFrom { get; init; }
    public DateTimeOffset? CreatedAtTo { get; init; }
    public int Page { get; init; }
  }
}