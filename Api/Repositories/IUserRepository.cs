using Api.Models;
using Api.Models.Dtos;

namespace Api.Repositories {
  public interface IUserRepository {
    User GetByCredentials(LoginRequestDto user);
  }
}