using Blazeroids.Core;
using Blazeroids.Core.Components;

namespace Blazeroids.Web.Game
{
    public class Weapon : BaseComponent
    {
        private long _lastBulletFiredTime = 0;
        private long _fireRate = 500;

        public Weapon(GameObject owner) : base(owner)
        {
        }

        public void Shoot(GameContext game)
        {
            var canShoot = game.GameTime.TotalMilliseconds - _lastBulletFiredTime >= _fireRate;
            if (!canShoot)
                return;

            _lastBulletFiredTime = game.GameTime.TotalMilliseconds;

            // create bullet, set position and direction 
            // append bullet to player children
        }
    }
}