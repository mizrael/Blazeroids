using System;
using System.Drawing;
using System.Threading.Tasks;
using Blazor.Extensions.Canvas.Canvas2D;

namespace Blazeroids.Core.Components
{
    public class BoundingBoxComponent : BaseComponent
#if DEBUG
    //    , IRenderable
#endif
    {
        private readonly TransformComponent _transform; 
        private Rectangle _boundingBox;
        private Size _halfSize;

        private BoundingBoxComponent(GameObject owner) : base(owner)
        {
            _transform = owner.Components.Get<TransformComponent>();
        }

        public void SetSize(Size size)
        {
            _boundingBox.Size = size;
            _halfSize = size / 2;
        }

        public override async ValueTask Update(GameContext game)
        {
            _boundingBox.X = (int)_transform.World.Position.X - _halfSize.Width;
            _boundingBox.Y = (int)_transform.World.Position.Y - _halfSize.Height;
        }

        public async ValueTask Render(GameContext game, Canvas2DContext context)
        {
            var tmpW = context.LineWidth;
            var tmpS = context.StrokeStyle;

            await context.BeginPathAsync();
            await context.SetStrokeStyleAsync("rgb(255,255,0)");
            await context.SetLineWidthAsync(3);
            await context.StrokeRectAsync(_boundingBox.X, _boundingBox.Y,
                _boundingBox.Width,
                _boundingBox.Height);
            
            await context.SetStrokeStyleAsync(tmpS);
            await context.SetLineWidthAsync(tmpW);
        }
    }
}