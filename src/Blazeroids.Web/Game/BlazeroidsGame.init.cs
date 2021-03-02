using System;
using System.Drawing;
using System.Numerics;
using System.Threading.Tasks;
using Blazeroids.Core;
using Blazeroids.Core.Assets;
using Blazeroids.Core.Components;
using Blazeroids.Core.GameServices;
using Blazeroids.Web.Game.Components;
using Blazor.Extensions;
using Blazeroids.Core.Utils;
using Blazor.Extensions.Canvas.Canvas2D;

namespace Blazeroids.Web.Game
{

    public partial class BlazeroidsGame : GameContext
    {
        private GameObject BuildBackground()
        {
            var background = new GameObject();
            
            var sprite = _assetsResolver.Get<Sprite>("assets/backgrounds/blue.png");

            var transform = background.Components.Add<TransformComponent>();
            if(_canvas.Width > sprite.Bounds.Width)
                transform.Local.Scale.X = 2f * (float)_canvas.Width / sprite.Bounds.Width;
            if (_canvas.Height > sprite.Bounds.Height)
                transform.Local.Scale.Y = 2f * (float)_canvas.Height / sprite.Bounds.Height;

            var renderer = background.Components.Add<RectRenderComponent>();
            renderer.Sprite = sprite;
            renderer.RepeatPattern = RepeatPattern.Repeat;
            renderer.LayerIndex = (int)RenderLayers.Background;

            return background;
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
                bulletSpriteRenderer.LayerIndex = (int)RenderLayers.Items;
                
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
            playerSpriteRenderer.LayerIndex = (int)RenderLayers.Player;
            
            var bbox = player.Components.Add<BoundingBoxComponent>();
            bbox.SetSize(sprite.Bounds.Size);

            var rigidBody = player.Components.Add<MovingBody>();
            
            var weapon = player.Components.Add<Weapon>();
            weapon.Spawner = bulletSpawner;

            var brain = player.Components.Add<PlayerBrain>();
            rigidBody.MaxSpeed = brain.Stats.EnginePower;
            
            brain.OnDeath += player =>
            {
                var explosion = _explosionsSpawner.Spawn();
                explosion.Enabled = true;
                var explosionTransform = explosion.Components.Get<TransformComponent>();
                explosionTransform.Local.Position = playerTransform.Local.Position;
                explosionTransform.Local.Position.X -= sprite.Bounds.Width;
                explosionTransform.Local.Position.Y -= sprite.Bounds.Height;
                
                _asteroidSpawnRate = _startAsteroidSpawnRate;
                    
                brain.Stats = PlayerStats.Default();
                playerTransform.Local.Position.X = _canvas.Width / 2;
                playerTransform.Local.Position.Y = _canvas.Height / 2;
                player.Enabled = true;
                _gameStats.ResetScore();
            };
            
            return player;
        }

        private GameObject BuidUI(Spawner bulletSpawner, GameObject player)
        {
            var ui = new GameObject();
            _gameStats = ui.Components.Add<GameStatsUIComponent>();
            _gameStats.LayerIndex = (int)RenderLayers.UI;

            #if DEBUG
            var debugStats = ui.Components.Add<DebugStatsUIComponent>();
            debugStats.BulletSpawner = bulletSpawner;
            debugStats.AsteroidsSpawner = _asteroidsSpawner;
            debugStats.LayerIndex = (int)RenderLayers.UI;
            #endif
            
            var playerStats = ui.Components.Add<PlayerStatsUIComponent>();
            playerStats.PlayerBrain = player.Components.Get<PlayerBrain>();
            playerStats.LayerIndex = (int)RenderLayers.UI;
            
            return ui;
        }

        private Spawner BuildExplosionsSpawner(){
            var animations = _assetsResolver.Get<AnimationCollection>("assets/animations/explosions.json");
            var explosionAnim = animations.GetAnimation("explosion1");

            var spawner = new Spawner(() =>
            {
                var explosion = new GameObject();
                explosion.Components.Add<TransformComponent>();

                var renderer = explosion.Components.Add<AnimatedSpriteRenderComponent>();
                renderer.Animation = explosionAnim;
                renderer.LayerIndex = (int)RenderLayers.Items;
                renderer.OnAnimationComplete += _ => explosion.Enabled = false;

                return explosion;
            }, explosion =>{
                var transform = explosion.Components.Get<TransformComponent>();

                transform.World.Reset(); 
                transform.Local.Reset();

                var renderer = explosion.Components.Add<AnimatedSpriteRenderComponent>();
                renderer.Reset();
            });

            spawner.Components.Add<TransformComponent>();

            return spawner;
        }

        private Spawner BuildAsteroidsSpawner(CollisionService collisionService)
        {
            var spriteNames = new[]
            {
                "meteorBrown_big1.png",
                "meteorBrown_big2.png",
                "meteorBrown_big3.png",
                "meteorBrown_big4.png",
                "meteorGrey_big1.png",
                "meteorGrey_big2.png",
                "meteorGrey_big3.png",
                "meteorGrey_big4.png",
            };
            int spriteIndex = 0;
            
            var spriteSheet = _assetsResolver.Get<SpriteSheet>("assets/sheet.json");
            
            var spawner = new Spawner(() =>
            {
                var asteroid = new GameObject();
                
                var transform = asteroid.Components.Add<TransformComponent>();
                
                var spriteRenderer = asteroid.Components.Add<SpriteRenderComponent>();
                var sprite = spriteSheet.Get(spriteNames[spriteIndex]);
                spriteIndex = spriteIndex + 1 % spriteNames.Length;
                spriteRenderer.Sprite = sprite;
                spriteRenderer.LayerIndex = (int)RenderLayers.Enemies;

                var bbox = asteroid.Components.Add<BoundingBoxComponent>();
                bbox.SetSize(sprite.Bounds.Size);
                collisionService.Add(bbox);
                
                var brain = asteroid.Components.Add<AsteroidBrain>();
                brain.Canvas = _canvas;
                brain.OnDeath += o =>
                {
                    _killedAsteroids++;
                    _gameStats.IncreaseScore();

                    var explosion = _explosionsSpawner.Spawn();
                    var explosionTransform = explosion.Components.Get<TransformComponent>();
                    explosionTransform.Local.Position = transform.Local.Position;
                    explosionTransform.Local.Position.X -= sprite.Bounds.Width;
                    explosionTransform.Local.Position.Y -= sprite.Bounds.Height;                     
                };

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