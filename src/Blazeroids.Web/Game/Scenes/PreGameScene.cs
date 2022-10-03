using Blazeroids.Core;
using Blazeroids.Core.Assets;
using Blazeroids.Core.Components;
using Blazeroids.Core.GameServices;
using Blazeroids.Web.Game.Components;
using Blazorex;
using System.Threading.Tasks;

namespace Blazeroids.Web.Game.Scenes
{
    public class PreGameScene : Scene
    {
        private readonly IAssetsResolver _assetsResolver;
        private readonly string _mainText;

        public PreGameScene(GameContext game, 
                            IAssetsResolver assetsResolver,
                            string mainText) : base(game)
        {
            _assetsResolver = assetsResolver;
            _mainText = mainText;
        }

        protected override ValueTask EnterCore()
        {
            var ui = BuidUI();
            this.Root.AddChild(ui);            

            var background = BuildBackground();
            this.Root.AddChild(background);
                        
            return base.EnterCore();
        }

        private GameObject BuidUI()
        {
            var ui = new GameObject();            
            
            var textComponent = ui.Components.Add<PreGameUIComponent>();
            textComponent.LayerIndex = (int)RenderLayers.UI;
            textComponent.MainText = _mainText;

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