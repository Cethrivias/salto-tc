using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces.Repositories;
using MediatR;

namespace Application.Mediatr.User
{
  public class GetUserByCredentialsQueryHandler : IRequestHandler<GetUserByCredentialsQuery, Domain.Models.User>
  {
    private readonly IUserRepository _userRepository;

    public GetUserByCredentialsQueryHandler(IUserRepository userRepository)
    {
      _userRepository = userRepository;
    }

    public Task<Domain.Models.User> Handle(GetUserByCredentialsQuery request, CancellationToken cancellationToken)
    {
      return _userRepository.GetByCredentials(request.Username, request.Password);
    }
  }
}