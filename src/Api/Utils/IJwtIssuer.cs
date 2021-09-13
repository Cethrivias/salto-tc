using Domain.Models;

namespace Api.Utils {
  public interface IJwtIssuer {
    string WriteToken(User user);
  }
}