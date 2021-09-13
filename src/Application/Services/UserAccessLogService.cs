using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Common.Interfaces.Repositories;
using Domain.Models;

namespace Application.Services {
  public class UserAccessLogService : IUserAccessLogService {
    private readonly IUserAccessLogRepository userAccessLogRepository;

    public UserAccessLogService(IUserAccessLogRepository userAccessLogRepository) {
      this.userAccessLogRepository = userAccessLogRepository;
    }

    public Task<List<UserAccessLog>> GetUserAccessLogs(
      int userId,
      int page,
      DateTimeOffset? createdAtFrom = null,
      DateTimeOffset? createdAtTo = null,
      int? lockId = null
    ) => userAccessLogRepository.GetUserAccessLogs(userId, page, createdAtFrom, createdAtTo, lockId);

    public Task<int> GetUserAccessLogsPages(
      int userId,
      DateTimeOffset? createdAtFrom = null,
      DateTimeOffset? createdAtTo = null,
      int? lockId = null
    ) => userAccessLogRepository.GetUserAccessLogsPages(userId, createdAtFrom, createdAtTo, lockId);

    public Task<UserAccessLog> LogUserAccess(int userId, int lockId) =>
      userAccessLogRepository.LogUserAccess(userId, lockId);
  }
}