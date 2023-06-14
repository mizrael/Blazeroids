using System.Drawing;
using System.Threading.Tasks;
using Blazeroids.Core;
using Blazeroids.Core.Assets;
using Blazeroids.Core.GameServices;
using Blazeroids.Web.Game.GameServices;
using Blazorex;

namespace Blazeroids.Web.Game
{
    public class BlazeroidsGame : GameContext
    {
        private readonly IAssetsResolver _assetsResolver;
        private RenderService _renderService;
        private InputService _inputService;

        public BlazeroidsGame(CanvasManagerBase canvasManager, 
                              IAssetsResolver assetsResolver,
                              ISoundService soundService) : base(canvasManager)
        {
            _assetsResolver = assetsResolver;

            this.AddService(soundService);
        }

        protected override async ValueTask InitGameAsync()
        {
            var canvasOptions = new CanvasCreationOptions()
            {
                Hidden = false,
                Width = this.Display.Size.Width,
                Height = this.Display.Size.Height,
                OnCanvasReady = async (canvas) =>
                {
                    _renderService = new RenderService(this, canvas.RenderContext);
                    this.AddService(_renderService);

                    _inputService = new InputService();
                    this.AddService(_inputService);

                    var collisionService = new CollisionService(this, new Size(64, 64));
                    this.AddService(collisionService);

                    this.SceneManager.AddScene(SceneNames.Welcome, new Scenes.PreGameScene(this, _assetsResolver, "Blazeroids!"));
                    this.SceneManager.AddScene(SceneNames.GameOver, new Scenes.PreGameScene(this, _assetsResolver, "Game over!"));
                    this.SceneManager.AddScene(SceneNames.Play, new Scenes.PlayScene(this, _assetsResolver));

                    await this.SceneManager.SetCurrentScene(SceneNames.Welcome);
                },
                OnFrameReady = async (timestamp) =>
                {
                    await this.StepAsync();
                },
                OnKeyDown = (keyCode) => 
                {
                    _inputService.Keyboard.SetKeyState((Keys)keyCode, ButtonState.States.Down);
                },
                OnKeyUp = (keyCode) => 
                {
                    _inputService.Keyboard.SetKeyState((Keys)keyCode, ButtonState.States.Up);
                },
                OnMouseMove = (coords) =>
                {
                    _inputService?.Mouse.SetPosition(coords);
                }
            };

            this.Display.CanvasManager.CreateCanvas("main", canvasOptions);           
        }
    }
}