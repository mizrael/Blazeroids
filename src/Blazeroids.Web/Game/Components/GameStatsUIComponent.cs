using System;
using System.Threading.Tasks;
using Blazeroids.Core;
using Blazeroids.Core.Components;
using Blazor.Extensions.Canvas.Canvas2D;

namespace Blazeroids.Web.Game.Components
{
    public class GameStatsUIComponent : Component, IRenderable
    {
        private int _score = 0;
        private int _maxScore = 0;

        private GameStatsUIComponent(GameObject owner) : base(owner)
        {
        }

        public async ValueTask Render(GameContext game, Canvas2DContext context)
        {
            await context.SetFillStyleAsync("#fff").ConfigureAwait(false);
            await context.SetTextAlignAsync(TextAlign.Left).ConfigureAwait(false);
            await context.SetFontAsync("18px verdana").ConfigureAwait(false);
            
            var hiScore = Math.Max(_score, _maxScore);
            var text = $"Score: {_score:###} Hi Score: {hiScore:###}";
            var textSize = await context.MeasureTextAsync(text).ConfigureAwait(false);
            var x = game.Display.Size.Width - textSize.Width - 50;
            
            await context.FillTextAsync(text, x, 50).ConfigureAwait(false);
        }

        public int LayerIndex { get; set; }

        public void IncreaseScore()
        {
            _score += 25;
        }

        public void ResetScore()
        {
            _maxScore = Math.Max(_score, _maxScore);
            _score = 0;
        }
    }
}