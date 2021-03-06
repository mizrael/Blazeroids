using System;
using System.Threading.Tasks;

namespace Blazeroids.Core.Components
{
    public class LambdaComponent : Component
    {
        private LambdaComponent(GameObject owner) : base(owner)
        {
        }

        protected override ValueTask UpdateCore(GameContext game)
        {
            return (OnUpdate is null) ? ValueTask.CompletedTask : OnUpdate(this.Owner, game);
        }

        public Func<GameObject, GameContext, ValueTask> OnUpdate;
    }
}