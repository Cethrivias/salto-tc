using MediatR;

namespace Application.Mediatr.Lock
{
  public class GetLockQuery : IRequest<Domain.Models.Lock>
  {
    public int LockId { get; init; }
    public int UserId { get; init; }
  }
}