using System.Threading.Tasks;
using Blazeroids.Core.Assets;

namespace Blazeroids.Core.Components
{
    public class SpriteRenderComponent : Component, IRenderable
    {
        private TransformComponent _transform;

        private SpriteRenderComponent(GameObject owner) : base(owner)
        {

        }

        protected override void Init()
        {
            _transform = Owner.Components.Get<TransformComponent>();
        }

        public async ValueTask Render(GameContext game, Blazorex.IRenderContext context)
        {
            if (!this.Owner.Enabled || this.Hidden)
                return;

            context.Save();

            context.Translate(_transform.World.Position.X, _transform.World.Position.Y);
            context.Rotate(_transform.World.Rotation);
            context.Scale(_transform.World.Scale.X, _transform.World.Scale.Y);

            context.DrawImage(Sprite.ElementRef,
                Sprite.Bounds.X, Sprite.Bounds.Y,
                Sprite.Bounds.Width, Sprite.Bounds.Height,
                Sprite.Origin.X, Sprite.Origin.Y,
                -Sprite.Bounds.Width, -Sprite.Bounds.Height);

            context.Restore();
        }

        public SpriteBase Sprite { get; set; }
        public int LayerIndex { get; set; }
        public bool Hidden { get; set; }
    }
}