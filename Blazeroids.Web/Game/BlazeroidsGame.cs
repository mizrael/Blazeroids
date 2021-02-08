using System;
using System.Drawing;
using System.Numerics;
using System.Threading.Tasks;
using Blazeroids.Core;
using Blazeroids.Core.Assets;
using Blazeroids.Core.Components;
using Blazeroids.Core.GameServices;
using Blazeroids.Web.Game.Components;
using Blazeroids.Web.Game.GameObjects;
using Blazor.Extensions;
using Blazeroids.Core.Utils;

namespace Blazeroids.Web.Game
{
    public class BlazeroidsGame : GameContext
    {
        private readonly BECanvasComponent _canvas;
        private readonly IAssetsResolver _assetsResolver;

        private long _lastAsteroidSpawnTime = 0;
        private long _startAsteroidSpawnRate = 2000;
        private long _maxAsteroidSpawnRate = 500;
        private long _asteroidSpawnRate = 2000;
        private Spawner _asteroidsSpawner;
        private GameObject _player;

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
            
            var sceneGraph = new SceneGraph(this);
            this.AddService(sceneGraph);

            var bulletSpawner = BuildBulletSpawner(collisionService);
            sceneGraph.Root.AddChild(bulletSpawner);

            _asteroidsSpawner = BuildAsteroidsSpawner(collisionService);
            sceneGraph.Root.AddChild(_asteroidsSpawner);

            _player = BuildPlayer(bulletSpawner);
            sceneGraph.Root.AddChild(_player);
            
            var ui = BuidUI(bulletSpawner, _player);
            sceneGraph.Root.AddChild(ui);

            var context = await _canvas.CreateCanvas2DAsync();
            var renderService = new RenderService(this, context);
            this.AddService(renderService);
        }

        private GameObject BuidUI(Spawner bulletSpawner, GameObject player)
        {
            var ui = new GameObject();
            var gameStats = ui.Components.Add<GameStatsUIComponent>();
            gameStats.BulletSpawner = bulletSpawner;
            gameStats.AsteroidsSpawner = _asteroidsSpawner;

            var playerStats = ui.Components.Add<PlayerStatsUIComponent>();
            playerStats.PlayerBrain = player.Components.Get<PlayerBrain>();
            
            return ui;
        }

        protected override ValueTask Update()
        {
            _asteroidSpawnRate = Math.Max(_asteroidSpawnRate - 1, _maxAsteroidSpawnRate);
            
            var canSpawnAsteroid = GameTime.TotalMilliseconds - _lastAsteroidSpawnTime >= _asteroidSpawnRate;
            if (canSpawnAsteroid)
            {
                _lastAsteroidSpawnTime = GameTime.TotalMilliseconds;
                _asteroidsSpawner.Spawn();
            }

            return base.Update();
        }

        private Spawner BuildBulletSpawner(CollisionService collisionService)
        {
            var spriteSheet = _assetsResolver.Get<SpriteSheet>("assets/sheet.json");
            
            var spawner = new Spawner(() =>
            {
                var bullet = new GameObject();
                bullet.Components.Add<TransformComponent>();

                var bulletSpriteRenderer = bullet.Components.Add<SpriteRenderComponent>();
                bulletSpriteRenderer.Sprite = spriteSheet.Get("fire01.png");

                var bulletBBox = bullet.Components.Add<BoundingBoxComponent>();
                bulletBBox.SetSize(bulletSpriteRenderer.Sprite.Bounds.Size);

                var speed = 7000f;
                
                var bulletRigidBody = bullet.Components.Add<MovingBody>();
                bulletRigidBody.MaxSpeed = speed;

                var brain = bullet.Components.Add<BulletBrain>();
                brain.Speed = speed;
                brain.Canvas = _canvas;

                collisionService.Add(bulletBBox);

                return bullet;
            }, bullet =>
            {
                bullet.Components.Get<MovingBody>().Reset();
                
                bullet.Components.Get<TransformComponent>().Local.Reset();
                bullet.Components.Get<TransformComponent>().World.Reset();
            });

            spawner.Components.Add<TransformComponent>();

            return spawner;
        }

        private GameObject BuildPlayer(Spawner bulletSpawner)
        {
            var player = new GameObject();

            var spriteSheet = _assetsResolver.Get<SpriteSheet>("assets/sheet.json");
            var sprite = spriteSheet.Get("playerShip2_green.png");

            var playerTransform = player.Components.Add<TransformComponent>();

            playerTransform.Local.Position.X = _canvas.Width / 2;
            playerTransform.Local.Position.Y = _canvas.Height / 2;

            var playerSpriteRenderer = player.Components.Add<SpriteRenderComponent>();
            playerSpriteRenderer.Sprite = sprite;

            var bbox = player.Components.Add<BoundingBoxComponent>();
            bbox.SetSize(sprite.Bounds.Size);

            var rigidBody = player.Components.Add<MovingBody>();
            rigidBody.MaxSpeed = 1000f;

            var weapon = player.Components.Add<Weapon>();
            weapon.Spawner = bulletSpawner;

            var brain = player.Components.Add<PlayerBrain>();
            brain.OnPlayerDead += player =>
            {
                _asteroidSpawnRate = _startAsteroidSpawnRate;
                    
                brain.Stats = PlayerStats.Default();
                playerTransform.Local.Position.X = _canvas.Width / 2;
                playerTransform.Local.Position.Y = _canvas.Height / 2;
                player.Enabled = true;
            };
            
            return player;
        }
        
        private Spawner BuildAsteroidsSpawner(CollisionService collisionService)
        {
            var spriteSheet = _assetsResolver.Get<SpriteSheet>("assets/sheet.json");
            var sprite = spriteSheet.Get("meteorBrown_big1.png");

            var spawner = new Spawner(() =>
            {
                var asteroid = new GameObject();
                
                asteroid.Components.Add<TransformComponent>();
                
                var spriteRenderer = asteroid.Components.Add<SpriteRenderComponent>();
                spriteRenderer.Sprite = sprite;

                var bbox = asteroid.Components.Add<BoundingBoxComponent>();
                bbox.SetSize(sprite.Bounds.Size);

                var brain = asteroid.Components.Add<AsteroidBrain>();
                brain.Canvas = _canvas;

                collisionService.Add(bbox);

                return asteroid;
            }, asteroid =>
            {
                var transform = asteroid.Components.Get<TransformComponent>();

                transform.World.Reset(); 
                transform.Local.Reset();
                
                transform.Local.Position.X = MathUtils.Random.NextBool() ? 0 : _canvas.Width;
                transform.Local.Position.Y = MathUtils.Random.NextBool() ? 0 : _canvas.Height;
                
                var brain = asteroid.Components.Get<AsteroidBrain>();
                var dir = _player.Components.Get<TransformComponent>().Local.Position - transform.Local.Position;
                brain.Direction = Vector2.Normalize(dir);
                brain.Speed = (float)MathUtils.Random.NextDouble(0.15, 0.5);

                //var w = (double)_canvas.Width;
                //var rx = MathUtils.Random.NextDouble(0, .4, .6, 1);
                //var tx = MathUtils.Normalize(rx, 0, 1, -1, 1);
                //transform.Local.Position.X = (float)(tx * w / 2.5 + w / 2);

                //var h = (double)_canvas.Height;
                //var ry = MathUtils.Random.NextDouble(0, .35, .65, 1);
                //var ty = MathUtils.Normalize(ry, 0, 1, -1, 1);
                //transform.Local.Position.Y = (float)(ty * h / 2.5 + h / 2);
            });

            spawner.Components.Add<TransformComponent>();

            return spawner;
        }
    }
}