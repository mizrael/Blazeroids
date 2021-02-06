using System.Threading.Tasks;

namespace Blazeroids.Core
{
    public interface IGameService
    {
        ValueTask Step();
    }
}