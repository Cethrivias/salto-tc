using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Libs.Database;
using Api.Models;
using LinqToDB;

namespace Api.Repositories {
  public class LockRepository : ILockRepository {
    private readonly MainDataConnection db;

    public LockRepository(MainDataConnection db) {
      this.db = db;
    }

    public Task<List<Lock>> GetAccessibleByUser(int userId) {
      var query = GetAccessibleByUserQuery(userId);

      return query.ToListAsync();
    }

    public Task<Lock> GetAccessibleByUser(int userId, int lockId) {
      var query = GetAccessibleByUserQuery(userId);
      query = from locks in query
              where locks.Id == lockId
              select locks;

      return query.FirstOrDefaultAsync();
    }

    private IQueryable<Lock> GetAccessibleByUserQuery(int userId) {
      return from locks in db.Locks
             join access in db.Access on locks.Id equals access.LockId
             join tags in db.Tags on access.TagId equals tags.Id
             join users in db.Users on tags.Id equals users.TagId
             where users.Id == userId
             select locks;
    }
  }
}