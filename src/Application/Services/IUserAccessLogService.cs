using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Models;

namespace Application.Services {
  public interface IUserAccessLogService {
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