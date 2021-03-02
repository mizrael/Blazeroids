using System.Threading.Tasks;

namespace Blazeroids.Core.GameServices
{
    public class SceneGraph : IGameService
    {
        private readonly GameContext _game;

        public SceneGraph(GameContext game)
        {
            _game = game;
            Root = new GameObject();
        }

        public void Step()
        {
            if (null == Root)
                return;
            
            Root.Update(_game);
        }
        
        public GameObject Root { get; }
    }
}