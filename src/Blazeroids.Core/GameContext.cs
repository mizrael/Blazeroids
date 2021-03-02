using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazeroids.Core.GameServices;

namespace Blazeroids.Core
{
    public abstract class GameContext
    {
        private bool _isInitialized = false;
        
        private Dictionary<Type, IGameService> _services = new();

        public T GetService<T>() where T : class, IGameService
        {
            _services.TryGetValue(typeof(T), out var service);
            return service as T;
        }
        
        protected void AddService(IGameService service)
        {
            if (service == null) 
                throw new ArgumentNullException(nameof(service));
            _services[service.GetType()] = service;
        }

        public async ValueTask Step()
        {
            if (!_isInitialized)
            {
                await this.Init();
                
                this.GameTime.Start();
                
                _isInitialized = true;
            }

            this.GameTime.Step();

            foreach (var service in _services.Values)
                service.Step();

            await this.Update();
        }

        protected abstract ValueTask Init();
        protected virtual ValueTask Update() => ValueTask.CompletedTask;

        public GameTime GameTime { get; } = new ();
        public Display Display { get; } = new ();

    }
}