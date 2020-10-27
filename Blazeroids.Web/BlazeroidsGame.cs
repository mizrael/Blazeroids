using System;
using System.Threading.Tasks;
using Blazeroids.Core;
using Blazeroids.Core.Assets;
using Blazeroids.Core.Components;
using Blazor.Extensions;
using Blazor.Extensions.Canvas.Canvas2D;

namespace Blazeroids.Web
{
    public class BlazeroidsGame : GameContext
    {
        private readonly Canvas2DContext _context;
        private readonly SceneGraph _sceneGraph;
        private readonly IAssetsResolver _assetsResolver;

        private BlazeroidsGame(Canvas2DContext context, IAssetsResolver assetsResolver)
        {
            _context = context;
            _assetsResolver = assetsResolver;
            _sceneGraph = new SceneGraph();
        }

        public static async ValueTask<BlazeroidsGame> Create(BECanvasComponent canvas, IAssetsResolver assetsResolver)
        {
            var context = await canvas.CreateCanvas2DAsync();
            var game = new BlazeroidsGame(context, assetsResolver);

            var fpsCounter = new GameObject();
            fpsCounter.Components.Add<FPSCounterComponent>();
            game._sceneGraph.Root.AddChild(fpsCounter);

            var player = new GameObject();
            var playerSprite = assetsResolver.Get<Sprite>("assets/playerShip2_green.png");
            var playerTransform = player.Components.Add<TransformComponent>();
            playerTransform.Local.Position.X = canvas.Width / 2 - playerSprite.Size.Width;
            playerTransform.Local.Position.Y = canvas.Height - playerSprite.Size.Height * 2;
            var playerSpriteRenderer = player.Components.Add<SpriteRenderComponent>();
            playerSpriteRenderer.Sprite = playerSprite;
            game._sceneGraph.Root.AddChild(player);

            var rand = new Random();
            for (var i = 0; i != 6; ++i)
                AddAsteroid(game, canvas, assetsResolver, rand);

            return game;
        }

        private static void AddAsteroid(BlazeroidsGame game, BECanvasComponent canvas, IAssetsResolver assetsResolver, Random rand)
        {
            var asteroid = new GameObject();

            var sprite = assetsResolver.Get<Sprite>("assets/meteorBrown_big1.png");
            
            var transform = asteroid.Components.Add<TransformComponent>();
            transform.Local.Position.X = rand.Next(sprite.Size.Width * 2, (int) canvas.Width - sprite.Size.Width * 2);
            transform.Local.Position.Y = rand.Next(sprite.Size.Height * 2, (int)(canvas.Height/4)*3);

            var spriteRenderer = asteroid.Components.Add<SpriteRenderComponent>();
            spriteRenderer.Sprite = sprite;

            asteroid.Components.Add<AsteroidBrainComponent>();

            game._sceneGraph.Root.AddChild(asteroid);
        }

        protected override async ValueTask Update()
        {
            await _sceneGraph.Update(this);
        }

        protected override async ValueTask Render()
        {
            await _context.ClearRectAsync(0, 0, this.Display.Size.Width, this.Display.Size.Height);
            
            await Render(_sceneGraph.Root);
        }

        private async ValueTask Render(GameObject node)
        {
            if (null == node)
                return;

            foreach(var component in node.Components)
                if (component is IRenderable renderable)
                    await renderable.Render(this, _context);

            foreach (var child in node.Children)
                await Render(child);
        }
    }
}