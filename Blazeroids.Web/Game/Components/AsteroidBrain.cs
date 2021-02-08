using System.Numerics;
using System.Threading.Tasks;
using Blazeroids.Core;
using Blazeroids.Core.Components;
using Blazeroids.Core.Utils;

namespace Blazeroids.Web.Game.Components
{
    public class AsteroidBrain : BaseComponent
    {
        private readonly TransformComponent _transform;
        private readonly BoundingBoxComponent _boundingBox;

        public float RotationSpeed = (float)MathUtils.Random.NextDouble(-0.005, 0.005);
        public Vector2 Direction;
        public float Speed = (float)MathUtils.Random.NextDouble(0.15, 0.5);

        private AsteroidBrain(GameObject owner) : base(owner)
        {   
            _transform = owner.Components.Get<TransformComponent>();
            _boundingBox = owner.Components.Get<BoundingBoxComponent>();
            _boundingBox.OnCollision += (sender, collidedWith) =>
            {
                if (!collidedWith.Owner.Components.TryGet<AsteroidBrain>(out var _)) 
                    this.Owner.Enabled = false;
            };
        }

        public override async ValueTask Update(GameContext game)
        {
            _transform.Local.Rotation += RotationSpeed * game.GameTime.ElapsedMilliseconds;
            _transform.Local.Position += Direction * Speed * game.GameTime.ElapsedMilliseconds;
        }
    }
}