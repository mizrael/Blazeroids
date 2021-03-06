using System.Threading.Tasks;
using Blazor.Extensions.Canvas.Canvas2D;

namespace Blazeroids.Core
{
    public interface IRenderable
    {
        ValueTask Render(GameContext game, Canvas2DContext context);
        
        int LayerIndex { get; set; }

        bool Hidden { get; set; }
    }
}