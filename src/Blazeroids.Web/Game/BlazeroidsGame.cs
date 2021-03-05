using System.Drawing;
using System.Threading.Tasks;
using Blazeroids.Core;
using Blazeroids.Core.Assets;
using Blazeroids.Core.GameServices;
using Blazeroids.Web.Game.GameServices;
using Blazor.Extensions;

namespace Blazeroids.Web.Game
{

    public class BlazeroidsGame : GameContext
    {
        private readonly IAssetsResolver _assetsResolver;

        public BlazeroidsGame(BECanvasComponent canvas, 
                              IAssetsResolver assetsResolver,
                              ISoundService soundService) : base(canvas)
        {
            _assetsResolver = assetsResolver;

            this.AddService(soundService);
        }

        protected override async ValueTask Init()
        {
            this.AddService(new InputService());

            var collisionService = new CollisionService(this, new Size(64, 64));
            this.AddService(collisionService);
            
            this.SceneManager.AddScene(SceneNames.Welcome, new Scenes.WelcomeScene(this, _assetsResolver));
            this.SceneManager.AddScene(SceneNames.Play, new Scenes.PlayScene(this, _assetsResolver));

            await this.SceneManager.SetCurrentScene(SceneNames.Welcome);
        }
    }
}