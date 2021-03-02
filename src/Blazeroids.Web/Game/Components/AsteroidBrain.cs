using System.Numerics;
using System.Threading.Tasks;
using Blazeroids.Core;
using Blazeroids.Core.Components;
using Blazeroids.Core.Utils;
using Blazor.Extensions;

namespace Blazeroids.Web.Game.Components
{
    public class AsteroidBrain : Component
    {
        private TransformComponent _transform;
        private BoundingBoxComponent _boundingBox;

        public float RotationSpeed = (float)MathUtils.Random.NextDouble(-0.005, 0.005);
        public Vector2 Direction;
        public float Speed = (float)MathUtils.Random.NextDouble(0.15, 0.5);
        public BECanvasComponent Canvas;

        public event OnDeathHandler OnDeath;
        public delegate void OnDeathHandler(GameObject asteroid);

        private AsteroidBrain(GameObject owner) : base(owner)
        {   
        }

        protected override void OnStart(){
            _transform = Owner.Components.Get<TransformComponent>();
            _boundingBox = Owner.Components.Get<BoundingBoxComponent>();
            _boundingBox.OnCollision += (sender, collidedWith) =>
            {
                if (collidedWith.Owner.Components.TryGet<AsteroidBrain>(out var _))
                    return;

                this.Owner.Enabled = false;
                this.OnDeath?.Invoke(this.Owner);
            };
        }

        protected override void OnUpdate(GameContext game)
        {
            _transform.Local.Rotation += RotationSpeed * game.GameTime.ElapsedMilliseconds;
            _transform.Local.Position += Direction * Speed * game.GameTime.ElapsedMilliseconds;

            var isOutScreen = _transform.World.Position.X < 0 ||
                              _transform.World.Position.Y < 0 ||
                              _transform.World.Position.X > this.Canvas.Width ||
                              _transform.World.Position.Y > this.Canvas.Height;
            if (isOutScreen)
                this.Owner.Enabled = false;
        }
    }
}