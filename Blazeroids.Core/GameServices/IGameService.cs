using System.Threading.Tasks;

namespace Blazeroids.Core.GameServices
{
    public interface IGameService
    {
        ValueTask Step();
    }
}