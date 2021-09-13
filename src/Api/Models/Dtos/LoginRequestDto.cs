using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;

namespace Api.Models.Dtos {
  public class LoginRequestDto {
    private string password;

    [Required]
    public string Username { get; set; }
    [Required]
    public string Password {
      get => password;
      set {
        var bytes = Encoding.ASCII.GetBytes(value);
        var hash = new SHA256Managed().ComputeHash(bytes);
        var hex = new StringBuilder();

        for (int i = 0; i < hash.Length; i++) {
          hex.Append(hash[i].ToString("x2"));
        }

        password = hex.ToString();
      }
    }
  }
}