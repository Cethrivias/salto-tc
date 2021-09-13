using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Models;

namespace Application.Common.Interfaces.Repositories {
  public interface ILockRepository {
    Task<List<Lock>> GetAccessibleByUser(int userId);
    Task<Lock> GetAccessibleByUser(int userId, int lockId);
  }
}