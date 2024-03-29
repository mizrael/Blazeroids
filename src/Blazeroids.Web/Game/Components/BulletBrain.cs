﻿using System.Threading.Tasks;
using Blazeroids.Core;
using Blazeroids.Core.Components;

namespace Blazeroids.Web.Game.Components
{
    public class BulletBrain : Component
    {
        private MovingBody _movingBody;
        private TransformComponent _transformComponent;
        private BoundingBoxComponent _boundingBox;

        public BulletBrain(GameObject owner) : base(owner)
        {

        }

        protected override void Init()
        {
            _movingBody = Owner.Components.Get<MovingBody>();
            _transformComponent = Owner.Components.Get<TransformComponent>();
            _boundingBox = Owner.Components.Get<BoundingBoxComponent>();
            _boundingBox.OnCollision += (sender, collidedWith) =>
            {
                if (collidedWith.Owner.Components.TryGet<AsteroidBrain>(out var _))
                    this.Owner.Enabled = false;
            };
        }

        protected override async ValueTask UpdateCore(GameContext game)
        {
            _movingBody.Thrust = this.Speed;

            var isOutScreen = _transformComponent.World.Position.X < 0 ||
                              _transformComponent.World.Position.Y < 0 ||
                              _transformComponent.World.Position.X > this.Display.Size.Width ||
                              _transformComponent.World.Position.Y > this.Display.Size.Height;
            if (isOutScreen)
                this.Owner.Enabled = false;
        }

        public float Speed { get; set; }
        public Display Display { get; set; }
    }
}