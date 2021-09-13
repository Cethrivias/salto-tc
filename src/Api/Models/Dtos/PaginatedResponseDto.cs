using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Api.Models.Dtos {
  public class PaginatedResponseDto<T> {
    [Required] public int Count => Data.Count;
    [Required] public List<T> Data { get; set; }
    [Required] public int Pages { get; set; }
    [Required] public int Page { get; set; }
  }
}