using System.Threading.Tasks;
using Blazeroids.Core.Assets;
using Blazorex;

namespace Blazeroids.Core.Components
{
    public class RectRenderComponent : Component, IRenderable
    {
        private TransformComponent _transform;
        private object _pattern;

        public RectRenderComponent(GameObject owner) : base(owner)
        {
        }

        protected override void Init()
        {
            _transform = Owner.Components.Get<TransformComponent>();            
        }

        public async ValueTask Render(GameContext game, Blazorex.IRenderContext context)
        {
            if (!this.Owner.Enabled || !this.Initialized)
                return;

            var oldPattern = context.FillStyle;
            _pattern ??= context.CreatePattern(Sprite.ElementRef, RepeatPattern);
            context.FillStyle = _pattern;

            var w = _transform.World.Scale.X * this.Sprite.Bounds.Width;
            var h = _transform.World.Scale.Y * this.Sprite.Bounds.Height;

            context.FillRect(_transform.World.Position.X, _transform.World.Position.X, w, h);

            context.FillStyle = oldPattern;
        }

        public SpriteBase Sprite { get; set; }
        public int LayerIndex { get; set; }
        public bool Hidden { get; set; }

        public RepeatPattern RepeatPattern { get; set; } = RepeatPattern.NoRepeat;
    }
}