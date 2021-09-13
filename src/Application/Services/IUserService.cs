using System.Threading.Tasks;
using Domain.Models;

namespace Application.Services {
  public interface IUserService {
    Task<User> GetByCredentials(string username, string password);
    Task<bool> HasAccess(int userId, int lockId);
  }
}