using System.Threading.Tasks;
using Application.Common.Interfaces.Repositories;
using Domain.Models;

namespace Application.Services {
  public class UserService : IUserService {
    private readonly IUserRepository userRepository;

    public UserService(IUserRepository userRepository) {
      this.userRepository = userRepository;
    }

    public Task<User> GetByCredentials(string username, string password) =>
      userRepository.GetByCredentials(username, password);

    public Task<bool> HasAccess(int userId, int lockId) => userRepository.HasAccess(userId, lockId);
  }
}