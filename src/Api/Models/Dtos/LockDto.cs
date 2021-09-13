using System.ComponentModel.DataAnnotations;

namespace Api.Models.Dtos {
  public class LockDto {
    [Required] public int Id { get; set; }
    [Required] public string Name { get; set; }
  }
}