using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Models;

namespace Application.Services {
  public interface ILockService {
    Task<List<Lock>> GetAccessibleByUser(int userId);
    Task<Lock> GetAccessibleByUser(int userId, int lockId);
  }
}