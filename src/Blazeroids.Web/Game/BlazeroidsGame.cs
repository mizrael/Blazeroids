using System.Drawing;
using System.Threading.Tasks;
using Blazeroids.Core;
using Blazeroids.Core.Assets;
using Blazeroids.Core.GameServices;
using Blazor.Extensions;

namespace Blazeroids.Web.Game
{

    public class BlazeroidsGame : GameContext
    {
        private readonly BECanvasComponent _canvas;
        private readonly IAssetsResolver _assetsResolver;

        public BlazeroidsGame(BECanvasComponent canvas, IAssetsResolver assetsResolver)
        {
            _canvas = canvas;
            _assetsResolver = assetsResolver;
        }

        protected override async ValueTask Init()
        {
            this.AddService(new InputService());

            var collisionService = new CollisionService(this, new Size(64, 64));
            this.AddService(collisionService);

            var context = await _canvas.CreateCanvas2DAsync();
            var renderService = new RenderService(this, context);
            this.AddService(renderService);

            var gameScene = new Scenes.GameScene(this, _canvas, _assetsResolver);
            this.SceneManager.AddScene("game", gameScene);
            await this.SceneManager.SetCurrentScene("game");
        }
    }
}