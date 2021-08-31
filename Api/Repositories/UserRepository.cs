using System.Linq;
using Api.Models;
using Api.Models.Dtos;

namespace Api.Repositories {
  public class UserRepository : IUserRepository {
    private User[] users = {
      new User() { Id = 1, Username = "Test User", Password = "Test Pass"  }
    };

    public User GetByCredentials(LoginRequestDto user) => users.FirstOrDefault(it => it.Username == user.Username && it.Password == user.Password);

  }
}