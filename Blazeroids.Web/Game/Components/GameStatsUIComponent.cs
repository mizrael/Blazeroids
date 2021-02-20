using System.Threading.Tasks;
using Blazeroids.Core;
using Blazeroids.Core.Components;
using Blazeroids.Web.Game.GameObjects;
using Blazor.Extensions.Canvas.Canvas2D;

namespace Blazeroids.Web.Game.Components
{
    public class GameStatsUIComponent : BaseComponent, IRenderable
    {
        private int startY = 50;
        private int y = 50;
        private int lineHeight = 30;
        private int x = 20; 
        
        private GameStatsUIComponent(GameObject owner) : base(owner)
        {
        }

        public async ValueTask Render(GameContext game, Canvas2DContext context)
        {
            var fps = 1000f / game.GameTime.ElapsedMilliseconds;

            await context.SetFillStyleAsync("green");
            await context.FillRectAsync(10, 50, 300, 200);
            
            await context.SetFontAsync("18px verdana");
            
            y = startY;
            await WriteLine($"Total game time (s): {game.GameTime.TotalMilliseconds / 1000}", context);
            await WriteLine($"Frame time (ms): {game.GameTime.ElapsedMilliseconds}", context);
            await WriteLine($"FPS: {fps:###}", context);

            if (AsteroidsSpawner is not null)
                await WriteLine($"Asteroids alive: {AsteroidsSpawner.Alive:###}", context);

            if (BulletSpawner is not null)
                await WriteLine($"Bullets spawned: {BulletSpawner.Alive:###}", context);
        }

        private async ValueTask WriteLine(string text, Canvas2DContext context)
        {
            y += lineHeight;
            await context.StrokeTextAsync(text, x, y).ConfigureAwait(false);
        }

        public Spawner BulletSpawner;
        public Spawner AsteroidsSpawner;

        public int LayerIndex { get; set; }
    }
}