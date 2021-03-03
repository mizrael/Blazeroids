using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazor.Extensions.Canvas.Canvas2D;

namespace Blazeroids.Core.GameServices
{
    public class RenderService : IGameService
    {
        private readonly GameContext _game;
        private readonly Canvas2DContext _context;

        public RenderService(GameContext game, Canvas2DContext context)
        {
            _game = game;
            _context = context;
        }

        public ValueTask Step() => ValueTask.CompletedTask;

        public ValueTask Render()
        {
            var layers = BuildLayers();
            return RenderFrame(layers);
        }

        private async ValueTask RenderFrame(SortedList<int, IList<IRenderable>> layers)
        {
            await _context.ClearRectAsync(0, 0, _game.Display.Size.Width, _game.Display.Size.Height)
                        .ConfigureAwait(false);

            await _context.BeginBatchAsync().ConfigureAwait(false);

            foreach (var layer in layers.Values)
                foreach (var renderable in layer)
                    await renderable.Render(_game, _context).ConfigureAwait(false);

            await _context.EndBatchAsync().ConfigureAwait(false);
        }

        private SortedList<int, IList<IRenderable>> BuildLayers()
        {
            var activeScene = _game.SceneManager.Current;
            var layers = new SortedList<int, IList<IRenderable>>();
            BuildLayers(activeScene.Root, layers);

            return layers;
        }

        private void BuildLayers(GameObject node, SortedList<int, IList<IRenderable>> layers)
        {
            if (null == node || !node.Enabled)
                return;

            foreach (var component in node.Components)
                if (component is IRenderable renderable && component.Initialized)
                {
                    if (!layers.ContainsKey(renderable.LayerIndex))
                        layers.Add(renderable.LayerIndex, new List<IRenderable>());
                    layers[renderable.LayerIndex].Add(renderable);
                }

            foreach (var child in node.Children)
                BuildLayers(child, layers);
        }
    }
}