using System;

namespace Blazeroids.Core.Components
{
    public abstract class Component
    {
        private bool _started = false;

        protected Component(GameObject owner)
        {
            this.Owner = owner ?? throw new ArgumentNullException(nameof(owner));
        }

        protected virtual void OnStart(){}

        protected virtual void OnUpdate(GameContext game){}

        public virtual void Update(GameContext game){
            if(!this.Owner.Enabled)
                return;
                
            if(!_started){
                _started = true;
                OnStart();
            }
            
            OnUpdate(game);
        } 

        public GameObject Owner { get; }
    }
}