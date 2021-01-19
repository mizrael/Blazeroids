using System;
using System.Threading.Tasks;
using Blazeroids.Core;
using Blazeroids.Core.Assets;
using Blazeroids.Core.Components;
using Blazeroids.Core.Utils;
using Blazor.Extensions;
using Blazor.Extensions.Canvas.Canvas2D;

namespace Blazeroids.Web.Game
{
    public class BlazeroidsGame : GameContext
    {
        private readonly Canvas2DContext _context;
        private readonly SceneGraph _sceneGraph;
        
        private BlazeroidsGame(Canvas2DContext context)
        {
            _context = context;
            _sceneGraph = new SceneGraph();
        }

        public static async ValueTask<BlazeroidsGame> Create(BECanvasComponent canvas, IAssetsResolver assetsResolver)
        {
            var context = await canvas.CreateCanvas2DAsync();
            var game = new BlazeroidsGame(context);

            //var fpsCounter = new GameObject();
            //fpsCounter.Components.Add<FPSCounterComponent>();
            //game._sceneGraph.Root.AddChild(fpsCounter);

            var bulletSpawner = BuildBulletSpawner(canvas, assetsResolver);
            game._sceneGraph.Root.AddChild(bulletSpawner);
            
            var player = BuildPlayer(canvas, assetsResolver, bulletSpawner);
            game._sceneGraph.Root.AddChild(player);

            var rand = new Random();
            for (var i = 0; i != 6; ++i)
                AddAsteroid(game, canvas, assetsResolver, rand);

            return game;
        }

        private static Spawner BuildBulletSpawner(BECanvasComponent canvas, IAssetsResolver assetsResolver)
        {
            var spriteSheet = assetsResolver.Get<SpriteSheet>("assets/sheet.json");
            
            var spawner = new Spawner(() =>
            {
                var bullet = new GameObject();
                bullet.Components.Add<TransformComponent>();

                var bulletSpriteRenderer = bullet.Components.Add<SpriteRenderComponent>();
                bulletSpriteRenderer.Sprite = spriteSheet.Get("fire01.png");

                var bulletBBox = bullet.Components.Add<BoundingBoxComponent>();
                bulletBBox.SetSize(bulletSpriteRenderer.Sprite.Bounds.Size);

                var speed = 7000f;
                
                var bulletRigidBody = bullet.Components.Add<MovingBodyComponent>();
                bulletRigidBody.MaxSpeed = speed;

                var brain = bullet.Components.Add<BulletBrainComponent>();
                brain.Speed = speed;
                brain.Canvas = canvas;

                return bullet;
            }, bullet =>
            {
                bullet.Components.Get<MovingBodyComponent>().Reset();
                
                bullet.Components.Get<TransformComponent>().Local.Reset();
                bullet.Components.Get<TransformComponent>().World.Reset();
            });

            spawner.Components.Add<TransformComponent>();

            return spawner;
        }

        private static GameObject BuildPlayer(BECanvasComponent canvas, 
            IAssetsResolver assetsResolver,
            Spawner bulletSpawner)
        {
            var player = new GameObject();

            var spriteSheet = assetsResolver.Get<SpriteSheet>("assets/sheet.json");
            var sprite = spriteSheet.Get("playerShip2_green.png");

            var playerTransform = player.Components.Add<TransformComponent>();

            playerTransform.Local.Position.X = canvas.Width / 2 - sprite.Bounds.Width;
            playerTransform.Local.Position.Y = canvas.Height / 2 - sprite.Bounds.Height * 2;

            var playerSpriteRenderer = player.Components.Add<SpriteRenderComponent>();
            playerSpriteRenderer.Sprite = sprite;

            var bbox = player.Components.Add<BoundingBoxComponent>();
            bbox.SetSize(sprite.Bounds.Size);

            var rigidBody = player.Components.Add<MovingBodyComponent>();
            rigidBody.MaxSpeed = 400f;

            var weapon = player.Components.Add<Weapon>();
            weapon.Spawner = bulletSpawner;

            player.Components.Add<PlayerBrain>();
            
            return player;
        }

        private static void AddAsteroid(BlazeroidsGame game, BECanvasComponent canvas, IAssetsResolver assetsResolver, Random rand)
        {
            var asteroid = new GameObject();

            var spriteSheet = assetsResolver.Get<SpriteSheet>("assets/sheet.json");
            var sprite = spriteSheet.Get("meteorBrown_big1.png");
            
            var transform = asteroid.Components.Add<TransformComponent>();
            transform.Local.Position.X = rand.Next(sprite.Bounds.Width * 2, (int) canvas.Width - sprite.Bounds.Width * 2);
            transform.Local.Position.Y = rand.Next(sprite.Bounds.Height * 2, (int)(canvas.Height/4)*3);

            var spriteRenderer = asteroid.Components.Add<SpriteRenderComponent>();
            spriteRenderer.Sprite = sprite;

            asteroid.Components.Add<AsteroidBrainComponent>();

            var bbox = asteroid.Components.Add<BoundingBoxComponent>();
            bbox.SetSize(sprite.Bounds.Size);

            game._sceneGraph.Root.AddChild(asteroid);
        }

        protected override async ValueTask Update()
        {
            await _sceneGraph.Update(this);
        }

        protected override async ValueTask Render()
        {
            await _context.ClearRectAsync(0, 0, this.Display.Size.Width, this.Display.Size.Height);

            await _context.BeginBatchAsync();
            await Render(_sceneGraph.Root);
            await _context.EndBatchAsync();
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