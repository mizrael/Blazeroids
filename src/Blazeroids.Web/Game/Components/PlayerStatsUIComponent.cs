using System.Threading.Tasks;
using Blazeroids.Core;
using Blazeroids.Core.Components;
using Blazor.Extensions.Canvas.Canvas2D;

namespace Blazeroids.Web.Game.Components
{
    public class PlayerStatsUIComponent : Component, IRenderable
    {
        private PlayerStatsUIComponent(GameObject owner) : base(owner)
        {
        }

        public async ValueTask Render(GameContext game, Canvas2DContext context)
        {
            int maxWidth = 300;
            int maxHeight = 30;
            int bottomOffset = 20;
            int rightOffset = 20;

            float healthRatio = (float) this.PlayerBrain.Stats.Health / this.PlayerBrain.Stats.MaxHealth;
            int width = (int)(healthRatio * maxWidth);

            int x = game.Display.Size.Width - width - rightOffset;
            int y = game.Display.Size.Height - maxHeight - bottomOffset;

            var color = healthRatio > .5 ? "green" : "red";

            await context.SetFillStyleAsync(color);
            await context.FillRectAsync(x, y, width, maxHeight);
        }

        public PlayerBrain PlayerBrain { get; set; }
        public int LayerIndex { get; set; }
    }
}