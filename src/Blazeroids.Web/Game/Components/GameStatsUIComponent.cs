using System;
using System.Threading.Tasks;
using Blazeroids.Core;
using Blazeroids.Core.Components;

namespace Blazeroids.Web.Game.Components
{
    public class GameStatsUIComponent : Component, IRenderable
    {
        private int _score = 0;
        private int _maxScore = 0;

        private GameStatsUIComponent(GameObject owner) : base(owner)
        {
        }

        public async ValueTask Render(GameContext game, Blazorex.IRenderContext context)
        {
            context.FillStyle = "#fff";
            context.TextAlign = Blazorex.TextAlign.Left;
            context.Font = "18px verdana"; ;
            
            var hiScore = Math.Max(_score, _maxScore);
            var text = $"Score: {_score:###} Hi Score: {hiScore:###}";
            var textSize = context.MeasureText(text);
            var x = game.Display.Size.Width - textSize.Width - 50;
            
             context.FillText(text, x, 50);
        }

        public int LayerIndex { get; set; }

        public bool Hidden {get;set;}

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