using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazeroids.Core.GameServices;

namespace Blazeroids.Core
{
    public abstract class GameContext
    {
        private bool _isInitialized = false;

        private Dictionary<Type, IGameService> _servicesMap = new();
        private List<IGameService> _services = new();

        protected GameContext()
        {
            this.SceneManager = new SceneManager(this);
            this.AddService(this.SceneManager);
        }

        public T GetService<T>() where T : class, IGameService
        {
            _servicesMap.TryGetValue(typeof(T), out var service);

            return service as T;
        }

        protected void AddService(IGameService service)
        {
            if (service == null)
                throw new ArgumentNullException(nameof(service));
            var serviceType = service.GetType();
            if (_servicesMap.ContainsKey(serviceType))
                throw new ArgumentException($"there is already a service of type '{serviceType.Name}'");
            _services.Add(service);
            _servicesMap[serviceType] = service;
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

            foreach (var service in _services)
                await service.Step();

            await this.Update();
        }

        protected abstract ValueTask Init();
        protected virtual ValueTask Update() => ValueTask.CompletedTask;

        public GameTime GameTime { get; } = new();
        public Display Display { get; } = new();

        public SceneManager SceneManager { get; }

    }
}