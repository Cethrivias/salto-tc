using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Models;

namespace Api.Repositories {
  public interface ILockRepository {
    Task<List<Lock>> GetAccessibleByUser(int userId);
    Task<Lock> GetAccessibleByUser(int userId, int lockId);
  }
}