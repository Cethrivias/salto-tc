using System.Linq;
using System.Threading.Tasks;
using Api.Core.Database;
using Api.Models;
using Api.Models.Dtos;
using LinqToDB;

namespace Api.Repositories {
  public class UserRepository : IUserRepository {

    private MainDataConnection db;

    public UserRepository(MainDataConnection db) {
      this.db = db;
    }

    public Task<User> GetByCredentials(LoginRequestDto user) {
      var query = from users in db.Users
                  where users.Username == user.Username && users.Password == user.Password
                  select users;

      return query.FirstOrDefaultAsync();
    }
  }
}