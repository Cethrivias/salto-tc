using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Core.Database;
using Api.Models;
using LinqToDB;

namespace Api.Repositories {
  public class UserAccessLogRepository : IUserAccessLogRepository {
    private readonly MainDataConnection db;

    private readonly int pageSize = 10;

    public UserAccessLogRepository(MainDataConnection db) {
      this.db = db;
    }

    public Task<List<UserAccessLog>> GetUserAccessLogs(int userId, int page) {
      var query = from logs in db.UsersAccessLogs
                  join access in db.Access on logs.LockId equals access.LockId
                  join tags in db.Tags on access.TagId equals tags.Id
                  join users in db.Users on tags.Id equals users.TagId
                  where logs.UserId == userId
                        && users.Id == userId
                  orderby logs.CreatedAt descending
                  select logs;

      return query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
    }


    public async Task<int> GetUserAccessLogsPages(int userId) {
      var query = from logs in db.UsersAccessLogs
                  join access in db.Access on logs.LockId equals access.LockId
                  join tags in db.Tags on access.TagId equals tags.Id
                  join users in db.Users on tags.Id equals users.TagId
                  where logs.UserId == userId
                        && users.Id == userId
                  select logs;
      var count = await query.CountAsync();

      var pages = (decimal)count / (decimal)pageSize;

      return (int)Math.Ceiling(pages);
    }

    public async Task<UserAccessLog> LogUserAccess(int userId, int lockId) {
      var userAccessLog = new UserAccessLog {
        LockId = lockId,
        UserId = userId
      };

      userAccessLog.Id = await db.InsertWithInt32IdentityAsync(userAccessLog);

      return userAccessLog;
    }
  }
}