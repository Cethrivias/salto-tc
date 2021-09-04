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

    public Task<User> GetByCredentials(LoginRequestDto credentials) {
      var query = from users in db.Users
                  where users.Username == credentials.Username && users.Password == credentials.Password
                  select users;

      return query.FirstOrDefaultAsync();
    }

    public async Task<bool> HasAccess(int userId, int lockId) {
      var query = from users in db.Users
                  join tags in db.Tags on users.TagId equals tags.Id
                  join access in db.Access on tags.Id equals access.TagId
                  join locks in db.Locks on access.LockId equals locks.Id
                  where users.Id == userId && locks.Id == lockId
                  select locks;
      var result = await query.CountAsync();

      return result != 0;
    }
  }
}