using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Common.Interfaces.Repositories;
using Domain.Models;

namespace Application.Services {
  public class LockService : ILockService {
    private readonly ILockRepository lockRepository;

    public LockService(ILockRepository lockRepository) {
      this.lockRepository = lockRepository;
    }

    public Task<List<Lock>> GetAccessibleByUser(int userId) => lockRepository.GetAccessibleByUser(userId);
    public Task<Lock> GetAccessibleByUser(int userId, int lockId) => lockRepository.GetAccessibleByUser(userId, lockId);
  }
}