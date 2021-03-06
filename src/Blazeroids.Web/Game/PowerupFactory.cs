using System;
using System.Threading.Tasks;
using Blazeroids.Core;
using Blazeroids.Core.Assets;
using Blazeroids.Core.Components;
using Blazeroids.Core.GameServices;
using Blazeroids.Web.Game.Components;

namespace Blazeroids.Web.Game
{
    public class PowerupFactory
    {
        private readonly SpriteSheet _spriteSheet;
        private readonly CollisionService _collisionService;

        public PowerupFactory(SpriteSheet spriteSheet, CollisionService collisionService)
        {
            _spriteSheet = spriteSheet ?? throw new ArgumentNullException(nameof(spriteSheet));
            _collisionService = collisionService ?? throw new ArgumentNullException(nameof(collisionService));
        }

        public GameObject Create()
        {
            var powerup = new GameObject();
            var transform = powerup.Components.Add<TransformComponent>();

            var sprite = _spriteSheet.Get("powerupGreen_shield.png");
            var spriteRenderer = powerup.Components.Add<SpriteRenderComponent>();
            spriteRenderer.Sprite = sprite;
            spriteRenderer.LayerIndex = (int)RenderLayers.Items;

            var bbox = powerup.Components.Add<BoundingBoxComponent>();
            bbox.SetSize(sprite.Bounds.Size);
            _collisionService.Add(bbox);

            bbox.OnCollision += (_, with) =>
            {
                if (!with.Owner.Components.TryGet<PlayerBrain>(out var playerBrain))
                    return;
                playerBrain.Stats.Health = playerBrain.Stats.MaxHealth;
                powerup.Enabled = false;
                powerup.Parent?.RemoveChild(powerup);
            };

            var lambdaComp = powerup.Components.Add<LambdaComponent>();
            lambdaComp.OnUpdate = (_, game) =>{
                var dt = (float)game.GameTime.TotalMilliseconds * 0.004f;
                transform.Local.Position.Y += MathF.Sin(dt);
                transform.Local.Position.X += MathF.Cos(dt);

                return ValueTask.CompletedTask;
            };

            return powerup;
        }
    }
}