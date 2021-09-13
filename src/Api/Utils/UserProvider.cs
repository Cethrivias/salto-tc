using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Api.Utils {
  public class UserProvider : IUserProvider {
    private readonly IHttpContextAccessor httpContextAccessor;

    public UserProvider(IHttpContextAccessor httpContextAccessor) {
      this.httpContextAccessor = httpContextAccessor;
    }

    public int UserId {
      get {
        var context = httpContextAccessor.HttpContext;
        var nameIdentifier = context.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        var userId = int.Parse(nameIdentifier);

        return userId;
      }
    }
  }
}