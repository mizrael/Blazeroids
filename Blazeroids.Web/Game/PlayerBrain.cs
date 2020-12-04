using System.Drawing;
using System.Threading.Tasks;
using Blazeroids.Core;
using Blazeroids.Core.Components;

namespace Blazeroids.Web.Game
{
    public class PlayerBrain : BaseComponent
    {
        private readonly MovingBodyComponent _movingBody;
        private readonly TransformComponent _transform;
        private readonly SpriteRenderComponent _spriteRender;
        private Weapon _weapon;
        private readonly Size _halfSize;
        
        private float _enginePower = 1000f;
        private float _rotationSpeed = 10f;

        public PlayerBrain(GameObject owner) : base(owner)
        {
            _movingBody = owner.Components.Get<MovingBodyComponent>();
            _transform = owner.Components.Get<TransformComponent>();
            _spriteRender = owner.Components.Get<SpriteRenderComponent>();
            _halfSize = _spriteRender.Sprite.Size / 2;
        }

        public override async ValueTask Update(GameContext game)
        {
            HandleMovement(game);

            HandleShooting(game);
        }

        private void HandleShooting(GameContext game)
        {
            var isShooting = (InputSystem.Instance.GetKeyState(Keys.Space).State == ButtonState.States.Down);
            if (isShooting)
            {
                _weapon ??= Owner.Components.Get<Weapon>();
                _weapon?.Shoot(game);
            }
        }

        private void HandleMovement(GameContext game)
        {
            if (_transform.World.Position.X < -_spriteRender.Sprite.Size.Width)
                _transform.Local.Position.X = game.Display.Size.Width + _halfSize.Width;
            else if (_transform.World.Position.X > game.Display.Size.Width + _spriteRender.Sprite.Size.Width)
                _transform.Local.Position.X = -_halfSize.Width;

            if (_transform.World.Position.Y < -_spriteRender.Sprite.Size.Height)
                _transform.Local.Position.Y = game.Display.Size.Height + _halfSize.Height;
            else if (_transform.World.Position.Y > game.Display.Size.Height + _spriteRender.Sprite.Size.Height)
                _transform.Local.Position.Y = -_halfSize.Height;

            if (InputSystem.Instance.GetKeyState(Keys.Right).State == ButtonState.States.Down)
                _movingBody.RotationSpeed = _rotationSpeed;
            else if (InputSystem.Instance.GetKeyState(Keys.Left).State == ButtonState.States.Down)
                _movingBody.RotationSpeed = -_rotationSpeed;
            else
                _movingBody.RotationSpeed = 0f;

            if (InputSystem.Instance.GetKeyState(Keys.Up).State == ButtonState.States.Down)
                _movingBody.Thrust = _enginePower;
            else if (InputSystem.Instance.GetKeyState(Keys.Down).State == ButtonState.States.Down)
                _movingBody.Thrust = -_enginePower;
            else
                _movingBody.Thrust = 0f;
        }
    }
}