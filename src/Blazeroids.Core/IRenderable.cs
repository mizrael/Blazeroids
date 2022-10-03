using System.Threading.Tasks;

namespace Blazeroids.Core
{
    public interface IRenderable
    {
        ValueTask Render(GameContext game, Blazorex.IRenderContext context);
        
        int LayerIndex { get; set; }

        bool Hidden { get; set; }
    }
}