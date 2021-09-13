using System.Threading.Tasks;

namespace Application.Core {
  public interface ILockOpener {
    Task Open(int lockId);
  }
}