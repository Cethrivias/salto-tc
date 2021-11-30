using MediatR;

namespace Application.Mediatr.User
{
  public class GetUserByCredentialsQuery : IRequest<Domain.Models.User>
  {
    public string Username { get; init; }
    public string Password { get; init; }
  }
}