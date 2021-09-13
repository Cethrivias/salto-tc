using System;
using System.ComponentModel.DataAnnotations;

namespace Api.Models.Dtos {
  public class UserAccessLogDto {
    [Required] public int Id { get; set; }
    [Required] public int LockId { get; set; }
    [Required] public DateTimeOffset CreatedAt { get; set; }
  }
}