using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazeroids.Core.GameServices;
using Blazeroids.Core.Web.Components;
using Blazor.Extensions;

namespace Blazeroids.Core
{
    public abstract class GameContext
    {
        private bool _isInitialized = false;

        private Dictionary<Type, IGameService> _servicesMap = new();
        private List<IGameService> _services = new();

        private RenderService _renderService;

        protected GameContext(CanvasManagerBase canvasManager)
        {
            this.Display = new Display(canvasManager);

            this.SceneManager = new SceneManager(this);
            this.AddService(this.SceneManager);
        }

        public T GetService<T>() where T : class, IGameService
        {
            _servicesMap.TryGetValue(typeof(T), out var service);

            return service as T;
        }

        public void AddService(IGameService service)
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
                await this.InitRenderer();
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

        private async ValueTask InitRenderer()
        {
            var canvas = await this.Display.CanvasManager.CreateCanvas("main");
            
            var context = await canvas.CreateCanvas2DAsync();
            _renderService = new RenderService(this, context);
            this.AddService(_renderService);
        }

        protected abstract ValueTask Init();
        protected virtual ValueTask Update() => ValueTask.CompletedTask;

        public GameTime GameTime { get; } = new();
        public Display Display { get; }
        public SceneManager SceneManager { get; private set; }
    }
}