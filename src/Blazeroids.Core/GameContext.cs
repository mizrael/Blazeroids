using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazeroids.Core.GameServices;
using Blazor.Extensions;

namespace Blazeroids.Core
{
    public abstract class GameContext
    {
        private bool _isInitialized = false;

        private Dictionary<Type, IGameService> _servicesMap = new();
        private List<IGameService> _services = new();

        private RenderService _renderService;
        private readonly BECanvasComponent _canvas;

        protected GameContext(Blazor.Extensions.BECanvasComponent canvas)
        {
            _canvas = canvas ?? throw new ArgumentNullException(nameof(canvas));
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
                await this.InitServices();
                await this.Init();

                this.GameTime.Start();

                _isInitialized = true;
            }

            this.GameTime.Step();

            foreach (var service in _services)
                await service.Step();

            await this.Update();

            await _renderService.Render();
        }

        private async ValueTask InitServices()
        {
            this.SceneManager = new SceneManager(this);
            this.AddService(this.SceneManager);

            var context = await _canvas.CreateCanvas2DAsync();
            _renderService = new RenderService(this, context);
            this.AddService(_renderService);
        }

        protected abstract ValueTask Init();
        protected virtual ValueTask Update() => ValueTask.CompletedTask;

        public GameTime GameTime { get; } = new();
        public Display Display { get; } = new();

        public SceneManager SceneManager { get; private set;}

    }
}