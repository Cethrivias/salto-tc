using System.Linq;
using System.Threading.Tasks;
using Application.Common.Interfaces.Repositories;
using Domain.Models;
using Infrastructure.Libs.Database;
using LinqToDB;

namespace Infrastructure.Repositories {
  public class UserRepository : IUserRepository {
    private MainDataConnection db;

    public UserRepository(MainDataConnection db) {
      this.db = db;
    }

    public Task<User> GetByCredentials(string username, string password) {
      var query = from users in db.Users
        where users.Username == username && users.Password == password
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