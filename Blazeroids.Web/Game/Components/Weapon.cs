using System;
using System.Numerics;
using Blazeroids.Core;
using Blazeroids.Core.Components;
using Blazeroids.Core.Utils;
using Blazeroids.Web.Game.GameObjects;

namespace Blazeroids.Web.Game.Components
{
    public class Weapon : BaseComponent
    {
        private long _lastBulletFiredTime = 0;
        private long _fireRate = 500;
        private TransformComponent _ownerTransform;
        
        public Weapon(GameObject owner) : base(owner)
        {
        }

        public void Shoot(GameContext game)
        {
            var canShoot = game.GameTime.TotalMilliseconds - _lastBulletFiredTime >= _fireRate;
            if (!canShoot)
                return;

            _ownerTransform ??= Owner.Components.Get<TransformComponent>();

            _lastBulletFiredTime = game.GameTime.TotalMilliseconds;

            var bullet = Spawner.Spawn();
            var bulletTransform = bullet.Components.Get<TransformComponent>();

            bulletTransform.Local.Rotation = _ownerTransform.Local.Rotation;

            bulletTransform.Local.Position = _ownerTransform.World.Position + Offset * _ownerTransform.Local.GetDirection();
        }

        public Spawner Spawner;

        public Vector2 Offset = new Vector2(0, -50);
    }
}