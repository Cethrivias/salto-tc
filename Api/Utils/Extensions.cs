using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Api.Utils.Extentions {
  public static class HttpContextExtensions {
    public static int GetUserId(this HttpContext context) {
      var nameIdentifier = context.User.FindFirst(ClaimTypes.NameIdentifier).Value;
      var userId = Int32.Parse(nameIdentifier);

      return userId;
    }
  }
}