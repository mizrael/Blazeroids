using System;
using System.Threading.Tasks;
using Blazeroids.Core;
using Blazeroids.Core.Components;


namespace Blazeroids.Web.Game.Components
{
    public class DebugStatsUIComponent : Component, IRenderable
    {
        private const int startY = 50;
        private int y = 50;
        private const int _lineHeight = 30;
        private int x = 20;
        private int _height = _lineHeight * 5 + startY/2;

        private DebugStatsUIComponent(GameObject owner) : base(owner)
        {
        }

        public async ValueTask Render(GameContext game, Blazorex.IRenderContext context)
        {
            var fps = 1000f / game.GameTime.ElapsedMilliseconds;

            context.FillStyle = "green";
            context.FillRect(10, 50, 300, _height);

            context.TextAlign = Blazorex.TextAlign.Left;
            context.FillStyle = "#fff";
            context.Font = "18px verdana";
            
            y = startY;

            await WriteLine($"Total game time (s): {game.GameTime.TotalMilliseconds / 1000}", context);
            await WriteLine($"Frame time (ms): {game.GameTime.ElapsedMilliseconds}", context);
            await WriteLine($"FPS: {fps:###}", context);

            if (AsteroidsSpawner is not null)
                await WriteLine($"Asteroids alive: {AsteroidsSpawner.Alive:###}", context);

            if (BulletSpawner is not null)
                await WriteLine($"Bullets spawned: {BulletSpawner.Alive:###}", context);
        }

        private async ValueTask WriteLine(string text, Blazorex.IRenderContext context)
        {
            y += _lineHeight;
            context.FillText(text, x, y);
        }

        public Spawner BulletSpawner;
        public Spawner AsteroidsSpawner;
        public int LayerIndex { get; set; }
        public bool Hidden { get; set; }
    }
}