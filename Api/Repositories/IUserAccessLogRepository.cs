using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Models;

namespace Api.Repositories {
  public interface IUserAccessLogRepository {
    Task<List<UserAccessLog>> GetUserAccessLogs(int userId, int page);
    Task<int> GetUserAccessLogsPages(int userId);
    Task<UserAccessLog> LogUserAccess(int userId, int lockId);
  }
}