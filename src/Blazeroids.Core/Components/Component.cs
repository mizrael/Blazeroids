using System;
using System.Threading.Tasks;

namespace Blazeroids.Core.Components
{
    public abstract class Component
    {
        private bool _initialized = false;

        protected Component(GameObject owner)
        {
            this.Owner = owner ?? throw new ArgumentNullException(nameof(owner));
        }

        protected virtual void Init(){}

        protected virtual ValueTask UpdateCore(GameContext game) => ValueTask.CompletedTask;

        public virtual ValueTask Update(GameContext game){
            if(!this.Owner.Enabled)
                return ValueTask.CompletedTask;
                
            if(!_initialized){                
                Init();
                _initialized = true;
            }
            
            return UpdateCore(game);
        } 

        public GameObject Owner { get; }
        public bool Initialized => _initialized;
    }
}