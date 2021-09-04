using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Models;

namespace Api.Repositories {
  public interface IUserAccessLogRepository {
    Task<List<UserAccessLog>> GetUserAccessLogs(
      int userId,
      int page,
      DateTimeOffset? createdAtFrom = null,
      DateTimeOffset? createdAtTo = null,
      int? lockId = null
    );
    Task<int> GetUserAccessLogsPages(
      int userId,
      DateTimeOffset? createdAtFrom = null,
      DateTimeOffset? createdAtTo = null,
      int? lockId = null
    );
    Task<UserAccessLog> LogUserAccess(int userId, int lockId);
  }
}