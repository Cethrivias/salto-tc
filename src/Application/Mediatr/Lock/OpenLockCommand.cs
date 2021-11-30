using Domain.Models;
using MediatR;

namespace Application.Mediatr.Lock
{
  public class OpenLockCommand: IRequest<Domain.Models.UserAccessLog>
  {
    public int UserId { get; init; }
    public int LockId { get; init; }
  }
}