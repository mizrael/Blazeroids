using System.Drawing;
using System.Threading.Tasks;
using Blazeroids.Core;
using Blazeroids.Core.Components;

namespace Blazeroids.Web.Game.Components
{
    public class PlayerBrain : BaseComponent
    {
        private readonly MovingBody _movingBody;
        private readonly TransformComponent _transform;
        private readonly SpriteRenderComponent _spriteRender;
        private readonly BoundingBoxComponent _boundingBox;
        
        private Weapon _weapon;
        private readonly Size _halfSize;
        
        private float _enginePower = 1000f;
        private float _rotationSpeed = 25f;

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
                    this.Owner.Enabled = false;
            };
        }

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
                _movingBody.RotationSpeed = _rotationSpeed;
            else if (inputService.GetKeyState(Keys.Left).State == ButtonState.States.Down)
                _movingBody.RotationSpeed = -_rotationSpeed;
            else
                _movingBody.RotationSpeed = 0f;

            if (inputService.GetKeyState(Keys.Up).State == ButtonState.States.Down)
                _movingBody.Thrust = _enginePower;
            else if (inputService.GetKeyState(Keys.Down).State == ButtonState.States.Down)
                _movingBody.Thrust = -_enginePower;
            else
                _movingBody.Thrust = 0f;
        }
    }
}