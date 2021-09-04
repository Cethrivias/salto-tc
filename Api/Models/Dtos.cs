using System.Collections.Generic;
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

  public class PaginatedAccessLogResponseDto : PaginatedResponseDto<UserAccessLog> { }

  public class PaginatedResponseDto<T> {
    [Required] public int count => data.Count;
    [Required] public List<T> data { get; set; }
    [Required] public int pages { get; set; }
    [Required] public int page { get; set; }
  }
}