using System.Threading.Tasks;

namespace Blazeroids.Core.Components
{
    public class TransformComponent : BaseComponent
    {
        private readonly Transform _local = Transform.Identity();
        private readonly Transform _world = Transform.Identity();

        private TransformComponent(GameObject owner) : base(owner)
        {
        }

        protected override async ValueTask OnUpdate(GameContext game)
        {
            _world.Clone(_local);
            
            if (null != Owner.Parent && Owner.Parent.Components.TryGet<TransformComponent>(out var parentTransform))
                _world.Position = _local.Position + parentTransform.World.Position;
        }

        public Transform Local => _local;
        public Transform World => _world;
    }
}