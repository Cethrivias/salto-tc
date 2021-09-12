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

  public class LoginResponseDto {
    public LoginResponseDto(string token) {
      this.Token = token;
    }
    [Required] public string Token { get; set; }
  }

  public class PaginatedAccessLogsDto : PaginatedResponseDto<UserAccessLogDto> { }

  public class PaginatedResponseDto<T> {
    [Required] public int Count => Data.Count;
    [Required] public List<T> Data { get; set; }
    [Required] public int Pages { get; set; }
    [Required] public int Page { get; set; }
  }

  public class UserAccessLogDto {
    [Required] public int Id { get; set; }
    [Required] public int LockId { get; set; }
    [Required] public DateTimeOffset CreatedAt { get; set; }
  }
}