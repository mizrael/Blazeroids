using System.Threading.Tasks;
using Blazeroids.Core;
using Blazeroids.Core.Components;
using Blazor.Extensions.Canvas.Canvas2D;

namespace Blazeroids.Web.Game.Components
{
    public class PlayerStatsUIComponent : Component, IRenderable
    {
        private const int _maxWidth = 200;
        private const int _maxHeight = 20;
        private const int _bottomOffset = 20;
        private const int _rightOffset = 20;

        private const string _shieldColor = "#4444FF";

        private PlayerStatsUIComponent(GameObject owner) : base(owner)
        {
        }

        public async ValueTask Render(GameContext game, Canvas2DContext context)
        {
            await RenderHealth(game, context);
            await RenderShield(game, context);
        }

        private async Task RenderShield(GameContext game, Canvas2DContext context)
        {
            float ratio = (float)this.PlayerBrain.Stats.ShieldHealth / this.PlayerBrain.Stats.ShieldMaxHealth;
            int width = (int)(ratio * _maxWidth);

            int x = game.Display.Size.Width - width - _rightOffset;
            int y = game.Display.Size.Height - _maxHeight - _bottomOffset - _maxHeight - 5;

            await context.SetFillStyleAsync(_shieldColor);
            await context.FillRectAsync(x, y, width, _maxHeight);
        }

        private async Task RenderHealth(GameContext game, Canvas2DContext context)
        {
            float ratio = (float)this.PlayerBrain.Stats.Health / this.PlayerBrain.Stats.MaxHealth;
            int width = (int)(ratio * _maxWidth);

            int x = game.Display.Size.Width - width - _rightOffset;
            int y = game.Display.Size.Height - _maxHeight - _bottomOffset;

            var color = ratio > .5 ? "green" : "red";

            await context.SetFillStyleAsync(color);
            await context.FillRectAsync(x, y, width, _maxHeight);
        }

        public PlayerBrain PlayerBrain { get; set; }
        public int LayerIndex { get; set; }
        public bool Hidden { get; set; }
    }
}