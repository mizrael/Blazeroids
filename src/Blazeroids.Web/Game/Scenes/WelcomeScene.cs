using Blazeroids.Core;
using Blazeroids.Core.Assets;
using Blazeroids.Core.Components;
using Blazeroids.Core.GameServices;
using Blazeroids.Web.Game.Components;
using Blazor.Extensions.Canvas.Canvas2D;
using System.Threading.Tasks;

namespace Blazeroids.Web.Game.Scenes
{
    public class WelcomeScene : Scene
    {
        private readonly IAssetsResolver _assetsResolver;

        public WelcomeScene(GameContext game, IAssetsResolver assetsResolver) : base(game)
        {
            _assetsResolver = assetsResolver;
        }

        public override ValueTask Enter()
        {
            var ui = BuidUI();
            this.Root.AddChild(ui);            

            var background = BuildBackground();
            this.Root.AddChild(background);
                        
            return base.Enter();
        }

        private GameObject BuidUI()
        {
            var ui = new GameObject();            
            
            var debugStats = ui.Components.Add<WelcomeUIComponent>();
            debugStats.LayerIndex = (int)RenderLayers.UI;

            return ui;
        }

        private GameObject BuildBackground()
        {
            var background = new GameObject();

            var sprite = _assetsResolver.Get<Sprite>("assets/backgrounds/blue.png");

            var transform = background.Components.Add<TransformComponent>();
            if (this.Game.Display.Size.Width > sprite.Bounds.Width)
                transform.Local.Scale.X = 2f * (float)this.Game.Display.Size.Width / sprite.Bounds.Width;
            if (this.Game.Display.Size.Height > sprite.Bounds.Height)
                transform.Local.Scale.Y = 2f * (float)this.Game.Display.Size.Height / sprite.Bounds.Height;

            var renderer = background.Components.Add<RectRenderComponent>();
            renderer.Sprite = sprite;
            renderer.RepeatPattern = RepeatPattern.Repeat;
            renderer.LayerIndex = (int)RenderLayers.Background;

            return background;
        }

    }
}