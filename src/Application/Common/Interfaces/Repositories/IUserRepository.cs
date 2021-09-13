using System.Threading.Tasks;
using Domain.Models;

namespace Application.Common.Interfaces.Repositories {
  public interface IUserRepository {
    Task<User> GetByCredentials(string username, string password);

    Task<bool> HasAccess(int userId, int lockId);
  }
}