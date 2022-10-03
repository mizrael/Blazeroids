using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blazeroids.Core.GameServices
{
    public class RenderService : IGameService
    {
        private readonly GameContext _game;
        private readonly Blazorex.IRenderContext _context;

        public RenderService(GameContext game, Blazorex.IRenderContext context)
        {
            _game = game;
            _context = context;
        }

        public ValueTask Step() 
        {
            var layers = BuildLayers();
            return RenderFrame(layers);
        }

        private async ValueTask RenderFrame(SortedList<int, IList<IRenderable>> layers)
        {
            _context.ClearRect(0, 0, _game.Display.Size.Width, _game.Display.Size.Height);

            foreach (var layer in layers.Values)
                foreach (var renderable in layer)
                    await renderable.Render(_game, _context).ConfigureAwait(false);
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
                if (component is IRenderable renderable &&
                    component.Initialized &&
                    !renderable.Hidden)
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