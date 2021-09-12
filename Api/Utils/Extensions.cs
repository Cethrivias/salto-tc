using System.Collections.Generic;
using Api.Models;
using Api.Models.Dtos;

namespace Api.Utils.Extentions {
  public static class UserAccessLogExtensions {
    public static UserAccessLogDto ToUserAccessLogDto(this UserAccessLog userAccessLog) => new UserAccessLogDto {
      Id = userAccessLog.Id,
      LockId = userAccessLog.LockId,
      CreatedAt = userAccessLog.CreatedAt
    };
  }

  public static class LockExtensions {
    public static LockDto ToLockDto(this Lock _lock) => new LockDto {
      Id = _lock.Id,
      Name = _lock.Name
    };
  }
}