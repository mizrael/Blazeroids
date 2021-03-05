using System.Numerics;
using System.Threading.Tasks;
using Blazeroids.Core;
using Blazeroids.Core.Components;
using Blazeroids.Web.Game.GameServices;

namespace Blazeroids.Web.Game.Components
{
    public class Weapon : Component
    {
        private long _lastBulletFiredTime = 0;
        private TransformComponent _ownerTransform;

        public Weapon(GameObject owner) : base(owner)
        {
        }

        protected override void Init()
        {
            _ownerTransform = Owner.Components.Get<TransformComponent>();
        }

        public void Shoot(GameContext game)
        {
            var canShoot = game.GameTime.TotalMilliseconds - _lastBulletFiredTime >= FireRate;
            if (!canShoot)
                return;

            _lastBulletFiredTime = game.GameTime.TotalMilliseconds;

            var bullet = Spawner.Spawn();
            var bulletTransform = bullet.Components.Get<TransformComponent>();

            bulletTransform.Local.Rotation = _ownerTransform.Local.Rotation;
            bulletTransform.Local.Position = GetBulletStartPosition();

            game.GetService<SoundService>().Play(Sounds.Laser);
        }

        private Vector2 GetBulletStartPosition() => _ownerTransform.World.Position +
                                                    _ownerTransform.Local.GetDirection() * Offset;

        public Spawner Spawner;

        public float Offset = -50f;
        private long FireRate = 150;
    }
}