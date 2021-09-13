using System.Threading.Tasks;

namespace Application.Core {

  public class LockOpener : ILockOpener {
    public Task Open(int lockId) {
      return Task.CompletedTask;
    }
  }
}