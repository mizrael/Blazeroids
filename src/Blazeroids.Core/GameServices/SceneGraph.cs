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

        public ValueTask Step()
        {
            if (null != Root)
                Root.Update(_game);
            return ValueTask.CompletedTask;
        }

        public GameObject Root { get; }
    }
}