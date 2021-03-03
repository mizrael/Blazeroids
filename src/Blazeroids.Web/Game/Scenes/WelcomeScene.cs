using Blazeroids.Core;
using Blazeroids.Core.GameServices;
using Blazeroids.Web.Game.Components;
using System.Threading.Tasks;

namespace Blazeroids.Web.Game.Scenes
{
    public class WelcomeScene : Scene
    {
        public WelcomeScene(GameContext game) : base(game)
        {
        }

        public override ValueTask Enter()
        {
            var ui = BuidUI();
            this.Root.AddChild(ui);
            return base.Enter();
        }

        private GameObject BuidUI()
        {
            var ui = new GameObject();            
            
            var debugStats = ui.Components.Add<StartScreenUIComponent>();
            debugStats.LayerIndex = (int)RenderLayers.UI;

            return ui;
        }

    }
}