using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Domain.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Api.Utils {
  public class JwtIssuer : IJwtIssuer {
    private readonly JwtHeader jwtHeader;

    public JwtIssuer(IConfiguration configuration) {
      var jwtSingingKey = configuration.GetValue<string>("JwtSigningKey");
      var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSingingKey));
      var singingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
      jwtHeader = new JwtHeader(singingCredentials);
    }

    public string WriteToken(User user) {
      var claims = new List<Claim> {
        new(ClaimTypes.Name, user.Username),
        new(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new(JwtRegisteredClaimNames.Exp, new DateTimeOffset(DateTime.Now.AddMinutes(60)).ToUnixTimeSeconds().ToString()),
      };

      var token = new JwtSecurityToken(jwtHeader, new JwtPayload(claims));

      return new JwtSecurityTokenHandler().WriteToken(token);
    }
  }
}