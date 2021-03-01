using System;
using System.Threading.Tasks;

namespace Blazeroids.Core.Components
{
    public abstract class BaseComponent : IComponent
    {
        protected BaseComponent(GameObject owner)
        {
            this.Owner = owner ?? throw new ArgumentNullException(nameof(owner));
        }

        //TODO: add an OnStart method

        public virtual ValueTask Update(GameContext game) => ValueTask.CompletedTask;

        public GameObject Owner { get; }
    }
}