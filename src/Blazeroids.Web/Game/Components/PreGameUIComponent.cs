using System.Threading.Tasks;
using Blazeroids.Core;
using Blazeroids.Core.Components;
using Blazeroids.Core.GameServices;


namespace Blazeroids.Web.Game.Components
{
    public class PreGameUIComponent : Component, IRenderable
    {
        private PreGameUIComponent(GameObject owner) : base(owner)
        {
        }

        public async ValueTask Render(GameContext game, Blazorex.IRenderContext context)
        {
            var y = (float)game.Display.Size.Height / 2;
            var x = (float)game.Display.Size.Width / 2;

            context.FillStyle = "#fff";
            context.Font = "40px verdana";
            context.TextAlign = Blazorex.TextAlign.Center;

            context.FillText(this.MainText, x, y);

            context.Font = "28px verdana";
            context.FillText("press Enter to start", x, y + 40);
        }

        protected override async ValueTask UpdateCore(GameContext game)
        {
            var inputService = game.GetService<InputService>();
            var canStart = (inputService.Keyboard.GetKeyState(Keys.Enter).State == ButtonState.States.Down);
            if (canStart)
                await game.SceneManager.SetCurrentScene(SceneNames.Play);
        }

        public int LayerIndex { get; set; }
        public bool Hidden { get; set; }

        public string MainText { get; set; } = "Blazeroids!";
    }
}