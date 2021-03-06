using System.Threading.Tasks;
using Blazeroids.Core;
using Blazeroids.Core.Components;
using Blazeroids.Core.GameServices;
using Blazor.Extensions.Canvas.Canvas2D;

namespace Blazeroids.Web.Game.Components
{
    public class PreGameUIComponent : Component, IRenderable
    {
        private PreGameUIComponent(GameObject owner) : base(owner)
        {
        }

        public async ValueTask Render(GameContext game, Canvas2DContext context)
        {
            var y = (double)game.Display.Size.Height / 2;
            var x = (double)game.Display.Size.Width / 2;

            await context.SetFillStyleAsync("#fff").ConfigureAwait(false);
            await context.SetFontAsync("40px verdana").ConfigureAwait(false);
            await context.SetTextAlignAsync(TextAlign.Center).ConfigureAwait(false);

            await context.FillTextAsync(this.MainText, x, y).ConfigureAwait(false);

            await context.SetFontAsync("28px verdana").ConfigureAwait(false);
            await context.FillTextAsync("press Enter to start", x, y + 40)
                        .ConfigureAwait(false);
        }

        protected override async ValueTask UpdateCore(GameContext game)
        {
            var inputService = game.GetService<InputService>();
            var canStart = (inputService.GetKeyState(Keys.Enter).State == ButtonState.States.Down);
            if (canStart)
                await game.SceneManager.SetCurrentScene(SceneNames.Play);
        }

        public int LayerIndex { get; set; }

        public string MainText { get; set; } = "Blazeroids!";
    }
}