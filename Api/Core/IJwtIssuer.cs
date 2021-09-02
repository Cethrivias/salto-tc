using Api.Models;

namespace Api.Core {
  public interface IJwtIssuer {
    string WriteToken(User user);
  }
}