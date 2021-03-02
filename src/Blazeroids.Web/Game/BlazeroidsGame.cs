using System;
using System.Drawing;
using System.Numerics;
using System.Threading.Tasks;
using Blazeroids.Core;
using Blazeroids.Core.Assets;
using Blazeroids.Core.Components;
using Blazeroids.Core.GameServices;
using Blazeroids.Web.Game.Components;
using Blazor.Extensions;
using Blazeroids.Core.Utils;
using Blazor.Extensions.Canvas.Canvas2D;

namespace Blazeroids.Web.Game
{

    public partial class BlazeroidsGame : GameContext
    {
        private readonly BECanvasComponent _canvas;
        private readonly IAssetsResolver _assetsResolver;

        private long _lastAsteroidSpawnTime = 0;
        private long _startAsteroidSpawnRate = 2000;
        private long _maxAsteroidSpawnRate = 500;
        private long _asteroidSpawnRate = 2000;
        private int _killedAsteroids = 0;
        private Spawner _asteroidsSpawner;
        private Spawner _explosionsSpawner;
        private GameObject _player;
        private GameStatsUIComponent _gameStats;

        public BlazeroidsGame(BECanvasComponent canvas, IAssetsResolver assetsResolver)
        {
            _canvas = canvas;
            _assetsResolver = assetsResolver;
        }

        protected override async ValueTask Init()
        {   
            this.AddService(new InputService());

            var collisionService = new CollisionService(this, new Size(64, 64));
            this.AddService(collisionService);
            
            var sceneGraph = new SceneGraph(this);
            this.AddService(sceneGraph);

            _explosionsSpawner = BuildExplosionsSpawner();
            sceneGraph.Root.AddChild(_explosionsSpawner);

            _asteroidsSpawner = BuildAsteroidsSpawner(collisionService);
            sceneGraph.Root.AddChild(_asteroidsSpawner);

            var bulletSpawner = BuildBulletSpawner(collisionService);
            sceneGraph.Root.AddChild(bulletSpawner);
            
            _player = BuildPlayer(bulletSpawner);
            sceneGraph.Root.AddChild(_player);

            var ui = BuidUI(bulletSpawner, _player);
            sceneGraph.Root.AddChild(ui);

            var background = BuildBackground();
            sceneGraph.Root.AddChild(background);
            
            var context = await _canvas.CreateCanvas2DAsync();
            var renderService = new RenderService(this, context);
            this.AddService(renderService);
        }

        protected override ValueTask Update()
        {
            _asteroidSpawnRate = Math.Max(_asteroidSpawnRate - 1, _maxAsteroidSpawnRate);

            var canSpawnAsteroid = GameTime.TotalMilliseconds - _lastAsteroidSpawnTime >= _asteroidSpawnRate;
            if (canSpawnAsteroid)
            {
                _lastAsteroidSpawnTime = GameTime.TotalMilliseconds;
                _asteroidsSpawner.Spawn();
            }

            return base.Update();
        }
    }
}