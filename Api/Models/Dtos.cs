using System.ComponentModel.DataAnnotations;

namespace Api.Models.Dtos {
  public class LoginRequestDto {
    [Required]
    public string Username { get; set; }
    [Required]
    public string Password { get; set; }
  }
  public class LoginResponseDto {
    public LoginResponseDto(string Token) {
      this.Token = Token;
    }
    [Required] public string Token { get; set; }
  }
}