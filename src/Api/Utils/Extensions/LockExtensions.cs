using Api.Models.Dtos;
using Domain.Models;

namespace Api.Utils.Extensions {
  public static class LockExtensions {
    public static LockDto ToLockDto(this Lock _lock) => new LockDto {
      Id = _lock.Id,
      Name = _lock.Name
    };
  }
}