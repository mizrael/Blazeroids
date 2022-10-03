using System.Drawing;
using System.Threading.Tasks;

namespace Blazeroids.Core.Components
{
    public class BoundingBoxComponent : Component
#if DEBUG
    //    , IRenderable
#endif
    {
        private TransformComponent _transform;
        private Rectangle _bounds;
        private Size _halfSize;

        private BoundingBoxComponent(GameObject owner) : base(owner)
        {

        }

        protected override void Init()
        {
            _transform = Owner.Components.Get<TransformComponent>();
        }

        public Rectangle Bounds => _bounds;

        public void SetSize(Size size)
        {
            _bounds.Size = size;
            _halfSize = size / 2;
        }

        protected override async ValueTask UpdateCore(GameContext game)
        {
            var x = (int)_transform.World.Position.X - _halfSize.Width;
            var y = (int)_transform.World.Position.Y - _halfSize.Height;

            var changed = _bounds.X != x || _bounds.Y != y;
            _bounds.X = x;
            _bounds.Y = y;

            if (changed)
                OnPositionChanged?.Invoke(this);
        }

        public async ValueTask Render(GameContext game, Blazorex.IRenderContext context)
        {
            var tmpW = context.LineWidth;
            var tmpS = context.StrokeStyle;

            context.BeginPath();

            context.StrokeStyle = "rgb(255,255,0)";
            context.LineWidth = 3;

            context.StrokeRect(_bounds.X, _bounds.Y,
                _bounds.Width,
                _bounds.Height);

            context.StrokeStyle = tmpS;
            context.LineWidth = tmpW;
        }

        public event OnPositionChangedHandler OnPositionChanged;
        public delegate void OnPositionChangedHandler(BoundingBoxComponent sender);

        public void CollideWith(BoundingBoxComponent other) => this.OnCollision?.Invoke(this, other);

        public event OnCollisionHandler OnCollision;
        public delegate void OnCollisionHandler(BoundingBoxComponent sender, BoundingBoxComponent collidedWith);
    }
}