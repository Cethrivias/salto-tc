using System.Collections.Generic;
using System;
using System.Security.Claims;
using Api.Models;
using Api.Models.Dtos;
using Microsoft.AspNetCore.Http;

namespace Api.Utils.Extentions {
  public static class HttpContextExtensions {
    public static int GetUserId(this HttpContext context) {
      var nameIdentifier = context.User.FindFirst(ClaimTypes.NameIdentifier).Value;
      var userId = Int32.Parse(nameIdentifier);

      return userId;
    }
  }

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