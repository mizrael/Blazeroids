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

        public float Speed = (float)MathUtils.Random.NextDouble(-0.005, 0.005);

        private AsteroidBrain(GameObject owner) : base(owner)
        {   
            _transform = owner.Components.Get<TransformComponent>();
            _boundingBox = owner.Components.Get<BoundingBoxComponent>();
            _boundingBox.OnCollision += (sender, collidedWith) =>
            {
                this.Owner.Enabled = false;
            };
        }

        public override async ValueTask Update(GameContext game)
        {
            _transform.Local.Rotation += Speed * game.GameTime.ElapsedMilliseconds;
        }
    }
}