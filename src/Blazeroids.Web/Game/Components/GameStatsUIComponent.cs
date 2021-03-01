using System;
using System.Threading.Tasks;
using Blazeroids.Core;
using Blazeroids.Core.Components;
using Blazor.Extensions.Canvas.Canvas2D;

namespace Blazeroids.Web.Game.Components
{
    public class GameStatsUIComponent : BaseComponent, IRenderable
    {
        private const int startY = 50;
        private int y = 50;
        private const int _lineHeight = 30;
        private int x = 20;
        private int _score = 0;
        private int _maxScore = 0;

#if DEBUG
        private int _height = _lineHeight * 7 + startY/2;
#else
        private int _height = _lineHeight * 2 + startY/2;
#endif
        private GameStatsUIComponent(GameObject owner) : base(owner)
        {
        }

        public async ValueTask Render(GameContext game, Canvas2DContext context)
        {
            var fps = 1000f / game.GameTime.ElapsedMilliseconds;

            await context.SetFillStyleAsync("green");
            await context.FillRectAsync(10, 50, 300, _height);

            await context.SetFontAsync("18px verdana");
            
            y = startY;

#if DEBUG

            await WriteLine($"Total game time (s): {game.GameTime.TotalMilliseconds / 1000}", context);
            await WriteLine($"Frame time (ms): {game.GameTime.ElapsedMilliseconds}", context);
            await WriteLine($"FPS: {fps:###}", context);

            if (AsteroidsSpawner is not null)
                await WriteLine($"Asteroids alive: {AsteroidsSpawner.Alive:###}", context);

            if (BulletSpawner is not null)
                await WriteLine($"Bullets spawned: {BulletSpawner.Alive:###}", context);
#endif
            
            await WriteLine($"Score: {_score:###}", context);
            await WriteLine($"Max Score: {Math.Max(_score, _maxScore):###}", context);
        }

        private async ValueTask WriteLine(string text, Canvas2DContext context)
        {
            y += _lineHeight;
            await context.StrokeTextAsync(text, x, y).ConfigureAwait(false);
        }

        public Spawner BulletSpawner;
        public Spawner AsteroidsSpawner;

        public int LayerIndex { get; set; }

        public void IncreaseScore()
        {
            _score += 25;
        }

        public void ResetScore()
        {
            _maxScore = Math.Max(_score, _maxScore);
            _score = 0;
        }
    }
}