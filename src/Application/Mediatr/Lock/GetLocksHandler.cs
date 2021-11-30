using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces.Repositories;
using MediatR;

namespace Application.Mediatr.Lock
{
  public class GetLocksHandler : IRequestHandler<GetLocksQuery, List<Domain.Models.Lock>>
  {
    private readonly ILockRepository _lockRepository;

    public GetLocksHandler(ILockRepository lockRepository)
    {
      _lockRepository = lockRepository;
    }

    public Task<List<Domain.Models.Lock>> Handle(GetLocksQuery request, CancellationToken cancellationToken) =>
      _lockRepository.GetAccessibleByUser(request.UserId);
  }
}