using System.ComponentModel.DataAnnotations;

namespace Api.Models.Dtos {
  public class LoginResponseDto {
    public LoginResponseDto(string token) {
      this.Token = token;
    }
    [Required] public string Token { get; set; }
  }
}