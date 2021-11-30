using System.Collections.Generic;

namespace Application.Models
{
  public class PaginatedResponse<T>
  {
    public IEnumerable<T> Data { get; init; }
    public int Page { get; init; }
    public int Pages { get; init; }
  }
}