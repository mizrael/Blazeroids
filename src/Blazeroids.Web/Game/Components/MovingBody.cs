using System;
using System.Numerics;
using System.Threading.Tasks;
using Blazeroids.Core;
using Blazeroids.Core.Components;
using Blazeroids.Core.Utils;

namespace Blazeroids.Web.Game.Components
{
    public class MovingBody : Component
    {
        #region Members

        private TransformComponent _transform = null;
        private Vector2 _velocity = Vector2.Zero;
        private float _rotationVelocity = 0f;

        #endregion Members

        public MovingBody(GameObject owner) : base(owner)
        {
        }

        protected override void OnStart()
        {
            _transform = Owner.Components.Get<TransformComponent>();
        }

        protected override void OnUpdate(GameContext game)
        {
            var dt = (float)game.GameTime.ElapsedMilliseconds / 1000;

            _rotationVelocity += RotationSpeed * dt;
            _rotationVelocity *= (1f - dt * RotationDrag);
            _transform.Local.Rotation += _rotationVelocity * dt;

            var dir = new Vector2(MathF.Sin(_transform.Local.Rotation), -MathF.Cos(_transform.Local.Rotation));

            var traction = dir * this.Thrust;

            var acceleration = traction / Mass;
            _velocity += acceleration * dt;
            _velocity *= (1 - dt * Drag);
            _velocity = Vector2Utils.ClampMagnitude(ref _velocity, MaxSpeed);

            _transform.Local.Position += _velocity * dt;
        }

        public void Reset()
        {
            this.Thrust = 0f;
            this.RotationSpeed = 0f;
            _rotationVelocity = 0;
            _velocity = Vector2.Zero;
        }

        #region Properties

        public float Thrust = 0f;
        public float MaxSpeed = 1f;
        public float Drag = 5f;

        public float RotationSpeed = 0f;
        public float RotationDrag = 5f;

        public float Mass = 1f;

        #endregion Properties

    }
}