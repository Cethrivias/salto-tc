using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces.Repositories;
using MediatR;

namespace Application.Mediatr.Lock
{
  public class GetLockQueryHandler : IRequestHandler<GetLockQuery, Domain.Models.Lock>
  {
    private readonly ILockRepository _lockRepository;

    public GetLockQueryHandler(ILockRepository lockRepository)
    {
      _lockRepository = lockRepository;
    }

    public Task<Domain.Models.Lock> Handle(GetLockQuery request, CancellationToken cancellationToken) =>
      _lockRepository.GetAccessibleByUser(request.UserId, request.LockId);
  }
}