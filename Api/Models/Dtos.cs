using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;

namespace Api.Models.Dtos {
  public class LockDto {
    [Required] public int Id { get; set; }
    [Required] public string Name { get; set; }
  }

  public class LoginRequestDto {
    private string _password;

    [Required]
    public string Username { get; set; }
    [Required]
    public string Password {
      get => _password;
      set {
        var bytes = Encoding.ASCII.GetBytes(value);
        var hash = new SHA256Managed().ComputeHash(bytes);
        var hex = new StringBuilder();

        for (int i = 0; i < hash.Length; i++) {
          hex.Append(hash[i].ToString("x2"));
        }

        _password = hex.ToString();
      }
    }
  }

  public class LoginResponseDto {
    public LoginResponseDto(string Token) {
      this.Token = Token;
    }
    [Required] public string Token { get; set; }
  }

  public class PaginatedAccessLogsDto : PaginatedResponseDto<UserAccessLogDto> { }

  public class PaginatedResponseDto<T> {
    [Required] public int count => data.Count;
    [Required] public List<T> data { get; set; }
    [Required] public int pages { get; set; }
    [Required] public int page { get; set; }
  }

  public class UserAccessLogDto {
    [Required] public int Id { get; set; }
    [Required] public int LockId { get; set; }
    [Required] public DateTimeOffset CreatedAt { get; set; }
  }
}