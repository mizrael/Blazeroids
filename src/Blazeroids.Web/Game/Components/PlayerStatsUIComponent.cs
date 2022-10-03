using System.Threading.Tasks;
using Blazeroids.Core;
using Blazeroids.Core.Components;

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

        public async ValueTask Render(GameContext game, Blazorex.IRenderContext context)
        {
            await RenderHealth(game, context);
            await RenderShield(game, context);
        }

        private async Task RenderShield(GameContext game, Blazorex.IRenderContext context)
        {
            float ratio = (float)this.PlayerBrain.Stats.ShieldHealth / this.PlayerBrain.Stats.ShieldMaxHealth;
            int width = (int)(ratio * _maxWidth);

            int x = game.Display.Size.Width - width - _rightOffset;
            int y = game.Display.Size.Height - _maxHeight - _bottomOffset - _maxHeight - 5;

            context.FillStyle = _shieldColor;
            context.FillRect(x, y, width, _maxHeight);
        }

        private async Task RenderHealth(GameContext game, Blazorex.IRenderContext context)
        {
            float ratio = (float)this.PlayerBrain.Stats.Health / this.PlayerBrain.Stats.MaxHealth;
            int width = (int)(ratio * _maxWidth);

            int x = game.Display.Size.Width - width - _rightOffset;
            int y = game.Display.Size.Height - _maxHeight - _bottomOffset;

            var color = ratio > .5 ? "green" : "red";

            context.FillStyle = color;
            context.FillRect(x, y, width, _maxHeight);
        }

        public PlayerBrain PlayerBrain { get; set; }
        public int LayerIndex { get; set; }
        public bool Hidden { get; set; }
    }
}