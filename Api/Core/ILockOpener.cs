using System.Threading.Tasks;

namespace Api.Core {
  public interface ILockOpener {
    Task Open(int lockId);
  }
}