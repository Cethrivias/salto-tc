using Api.Models.Dtos;
using Domain.Models;

namespace Api.Utils.Extensions {
  public static class UserAccessLogExtensions {
    public static UserAccessLogDto ToUserAccessLogDto(this UserAccessLog userAccessLog) => new UserAccessLogDto {
      Id = userAccessLog.Id,
      LockId = userAccessLog.LockId,
      CreatedAt = userAccessLog.CreatedAt
    };
  }
}