using System;
using System.Threading.Tasks;
using Blazeroids.Core;
using Blazeroids.Core.Assets;
using Blazeroids.Core.Components;
using Blazeroids.Core.GameServices;
using Blazeroids.Core.Utils;
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
            return MathUtils.Random.NextBool() ? CreateHealth() : CreateShield();
        }

        private GameObject CreateHealth()
            => CreateBase("powerupGreen_bolt.png", playerBrain =>
            {
                playerBrain.Stats.Health = playerBrain.Stats.MaxHealth;
            });

        private GameObject CreateShield()
            => CreateBase("powerupBlue_shield.png", playerBrain =>
            {
                playerBrain.Stats.ShieldHealth = playerBrain.Stats.ShieldMaxHealth;
            });

        private GameObject CreateBase(string spriteName, Action<PlayerBrain> onPlayerCollision)
        {
            var powerup = new GameObject();
            var transform = powerup.Components.Add<TransformComponent>();

            var sprite = _spriteSheet.Get(spriteName);
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

                powerup.Enabled = false;
                powerup.Parent?.RemoveChild(powerup);

                onPlayerCollision(playerBrain);
            };

            var offset = (float)MathUtils.Random.NextDouble(-5, 5);

            var lambdaComp = powerup.Components.Add<LambdaComponent>();
            lambdaComp.OnUpdate = (_, game) =>
            {
                float dt = (float)game.GameTime.TotalMilliseconds * 0.004f + offset;
                transform.Local.Position.Y += MathF.Sin(dt);
                transform.Local.Position.X += MathF.Cos(dt);

                return ValueTask.CompletedTask;
            };

            return powerup;
        }


    }
}