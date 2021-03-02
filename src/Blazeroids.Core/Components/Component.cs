using System;
using System.Threading.Tasks;

namespace Blazeroids.Core.Components
{
    public abstract class BaseComponent
    {
        private bool _started = false;

        protected BaseComponent(GameObject owner)
        {
            this.Owner = owner ?? throw new ArgumentNullException(nameof(owner));
        }

        protected virtual ValueTask OnStart() => ValueTask.CompletedTask;

        protected virtual ValueTask OnUpdate(GameContext game) => ValueTask.CompletedTask;

        public virtual async ValueTask Update(GameContext game){
            if(!_started){
                _started = true;
                await OnStart();
            }
            
            await OnUpdate(game);
        } 

        public GameObject Owner { get; }
    }
}