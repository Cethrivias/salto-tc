using System.Threading.Tasks;
using Api.Models;
using Api.Models.Dtos;

namespace Api.Repositories {
  public interface IUserRepository {
    Task<User> GetByCredentials(LoginRequestDto user);

    Task<bool> HasAccess(int userId, int lockId);
  }
}