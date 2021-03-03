using System;

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

        protected virtual void UpdateCore(GameContext game){}

        public virtual void Update(GameContext game){
            if(!this.Owner.Enabled)
                return;
                
            if(!_initialized){                
                Init();
                _initialized = true;
            }
            
            UpdateCore(game);
        } 

        public GameObject Owner { get; }
        public bool Initialized => _initialized;
    }
}