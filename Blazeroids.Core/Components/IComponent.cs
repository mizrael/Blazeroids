using System.Threading.Tasks;

namespace Blazeroids.Core.Components
{
    public interface IComponent
    {
        ValueTask Update(GameContext game);

        GameObject Owner { get; }
    }
}