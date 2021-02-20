using System.Threading.Tasks;
using Blazeroids.Core.Assets;
using Blazor.Extensions.Canvas.Canvas2D;

namespace Blazeroids.Core.Components
{
    public class RectRenderComponent : BaseComponent, IRenderable
    {
        private readonly TransformComponent _transform;
        
        public RectRenderComponent(GameObject owner) : base(owner)
        {
            _transform = owner.Components.Get<TransformComponent>();
        }

        public async ValueTask Render(GameContext game, Canvas2DContext context)
        {
            if (!this.Owner.Enabled)
                return;

            var oldPattern = context.FillStyle;

            var pattern = await context.CreatePatternAsync(Sprite.ElementRef, RepeatPattern);
            await context.SetFillStyleAsync(pattern);

            var w = _transform.World.Scale.X * this.Sprite.Bounds.Width;
            var h = _transform.World.Scale.Y * this.Sprite.Bounds.Height;

            await context.FillRectAsync(_transform.World.Position.X, _transform.World.Position.X, w, h);
            
            await context.SetFillStyleAsync(oldPattern);
        }

        public SpriteBase Sprite { get; set; }

        public RepeatPattern RepeatPattern { get; set; } = RepeatPattern.NoRepeat;
    }
}