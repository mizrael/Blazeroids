using System.Drawing;
using System.Threading.Tasks;
using Blazeroids.Core;
using Blazeroids.Core.Components;
using Blazeroids.Core.GameServices;

namespace Blazeroids.Web.Game.Components
{
    public struct PlayerStats
    {
        public float EnginePower;
        public float RotationSpeed;
        public int MaxHealth;
        public int Health;

        public static PlayerStats Default() => new ()
        {
            EnginePower = 2000f,
            RotationSpeed = 25f,
            Health = 10,
            MaxHealth = 10
        };
    }
    
    public class PlayerBrain : BaseComponent
    {
        private readonly MovingBody _movingBody;
        private readonly TransformComponent _transform;
        private readonly SpriteRenderComponent _spriteRender;
        private readonly BoundingBoxComponent _boundingBox;
        
        private Weapon _weapon;
        private readonly Size _halfSize;

        public PlayerStats Stats = PlayerStats.Default();

        public PlayerBrain(GameObject owner) : base(owner)
        {
            _movingBody = owner.Components.Get<MovingBody>();
            _transform = owner.Components.Get<TransformComponent>();
            _spriteRender = owner.Components.Get<SpriteRenderComponent>();
            _halfSize = _spriteRender.Sprite.Bounds.Size / 2;

            _boundingBox = owner.Components.Get<BoundingBoxComponent>();
            _boundingBox.OnCollision += (sender, collidedWith) =>
            {
                if (collidedWith.Owner.Components.TryGet<AsteroidBrain>(out var _))
                {
                    this.Stats.Health--;
                    if (0 == this.Stats.Health)
                    {
                        this.Owner.Enabled = false;
                        this.OnPlayerDead?.Invoke(this.Owner);
                    }
                }
            };
        }

        public event OnPlayerDeadHandler OnPlayerDead;
        public delegate void OnPlayerDeadHandler(GameObject player);

        public override async ValueTask Update(GameContext game)
        {
            var inputService = game.GetService<InputService>();
            HandleMovement(game, inputService);
            HandleShooting(game, inputService);
        }

        private void HandleShooting(GameContext game, InputService inputService)
        {
            var isShooting = (inputService.GetKeyState(Keys.Space).State == ButtonState.States.Down);
            if (isShooting)
            {
                _weapon ??= Owner.Components.Get<Weapon>();
                _weapon?.Shoot(game);
            }
        }

        private void HandleMovement(GameContext game, InputService inputService)
        {
            if (_transform.World.Position.X < -_spriteRender.Sprite.Bounds.Width)
                _transform.Local.Position.X = game.Display.Size.Width + _halfSize.Width;
            else if (_transform.World.Position.X > game.Display.Size.Width + _spriteRender.Sprite.Bounds.Width)
                _transform.Local.Position.X = -_halfSize.Width;

            if (_transform.World.Position.Y < -_spriteRender.Sprite.Bounds.Height)
                _transform.Local.Position.Y = game.Display.Size.Height + _halfSize.Height;
            else if (_transform.World.Position.Y > game.Display.Size.Height + _spriteRender.Sprite.Bounds.Height)
                _transform.Local.Position.Y = -_halfSize.Height;

            if (inputService.GetKeyState(Keys.Right).State == ButtonState.States.Down)
                _movingBody.RotationSpeed = this.Stats.RotationSpeed;
            else if (inputService.GetKeyState(Keys.Left).State == ButtonState.States.Down)
                _movingBody.RotationSpeed = -this.Stats.RotationSpeed;
            else
                _movingBody.RotationSpeed = 0f;

            if (inputService.GetKeyState(Keys.Up).State == ButtonState.States.Down)
                _movingBody.Thrust = this.Stats.EnginePower;
            else if (inputService.GetKeyState(Keys.Down).State == ButtonState.States.Down)
                _movingBody.Thrust = -this.Stats.EnginePower;
            else
                _movingBody.Thrust = 0f;
        }
    }
}