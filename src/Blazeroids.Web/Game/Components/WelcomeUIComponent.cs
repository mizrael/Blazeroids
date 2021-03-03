using System.Threading.Tasks;
using Blazeroids.Core;
using Blazeroids.Core.Components;
using Blazeroids.Core.GameServices;
using Blazor.Extensions.Canvas.Canvas2D;

namespace Blazeroids.Web.Game.Components
{
    public class WelcomeUIComponent : Component, IRenderable
    {
        private WelcomeUIComponent(GameObject owner) : base(owner)
        {
        }

        public async ValueTask Render(GameContext game, Canvas2DContext context)
        {
            await context.SetFillStyleAsync("#fff").ConfigureAwait(false);
            await context.SetFontAsync("40px verdana").ConfigureAwait(false);
            
            var text = "Blazeroids!";
            var textSize = await context.MeasureTextAsync(text).ConfigureAwait(false);
            var y = game.Display.Size.Height / 2;
            var x = game.Display.Size.Width / 2 - textSize.Width / 2;

            await context.FillTextAsync(text, x, y).ConfigureAwait(false);          

            await context.SetFontAsync("28px verdana").ConfigureAwait(false);
            text = "press Enter to start";
            textSize = await context.MeasureTextAsync(text).ConfigureAwait(false);
            y += 40;
            x = game.Display.Size.Width / 2 - textSize.Width / 2;
            await context.FillTextAsync(text, x, y)
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
    }
}