using System.Collections.Generic;
using MediatR;

namespace Application.Mediatr.Lock
{
  public class GetLocksQuery : IRequest<List<Domain.Models.Lock>>
  {
    public int UserId { get; init; }
  }
}