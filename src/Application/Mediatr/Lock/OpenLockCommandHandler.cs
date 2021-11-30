using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces.Repositories;
using Application.Core;
using Domain.Models;
using MediatR;

namespace Application.Mediatr.Lock
{
  public class OpenLockCommandHandler : IRequestHandler<OpenLockCommand, Domain.Models.UserAccessLog>
  {
    private readonly IUserRepository _userRepository;
    private readonly IUserAccessLogRepository _userAccessLogRepository;
    private readonly ILockOpener _lockOpener;

    public OpenLockCommandHandler(IUserRepository userRepository, ILockOpener lockOpener, IUserAccessLogRepository userAccessLogRepository)
    {
      _userRepository = userRepository;
      _lockOpener = lockOpener;
      _userAccessLogRepository = userAccessLogRepository;
    }

    public async Task<Domain.Models.UserAccessLog> Handle(OpenLockCommand request, CancellationToken cancellationToken)
    {
      var hasAccess = await _userRepository.HasAccess(request.UserId, request.LockId);

      if (!hasAccess) {
        return null;
      }

      await _lockOpener.Open(request.LockId);
      return await _userAccessLogRepository.LogUserAccess(request.UserId, request.LockId);
    }
  }
}