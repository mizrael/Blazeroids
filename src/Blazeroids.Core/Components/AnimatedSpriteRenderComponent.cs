using System.Threading.Tasks;
using Blazeroids.Core.Assets;

namespace Blazeroids.Core.Components
{
    public class AnimatedSpriteRenderComponent : Component, IRenderable
    {
        private TransformComponent _transform;

        private int _currFramePosX = 0;
        private int _currFramePosY = 0;
        private int _currFrameIndex = 0;
        private long _lastUpdate = 0;

        private AnimationCollection.Animation _animation;

        private AnimatedSpriteRenderComponent(GameObject owner) : base(owner)
        {
        }

        protected override void Init()
        {
            _transform = Owner.Components.Get<TransformComponent>();
        }

        protected override async ValueTask UpdateCore(GameContext game)
        {
            if (null == Animation || !this.Owner.Enabled)
                return;

            var needUpdate = (game.GameTime.TotalMilliseconds - _lastUpdate > 1000f / Animation.Fps);
            if (!needUpdate)
                return;

            _lastUpdate = game.GameTime.TotalMilliseconds;

            _currFramePosX += Animation.FrameSize.Width;
            if (_currFramePosX >= Animation.ImageSize.Width)
            {
                _currFramePosX = 0;
                _currFramePosY += Animation.FrameSize.Height;
            }

            if (_currFramePosY >= Animation.ImageSize.Height)
                _currFramePosY = 0;

            _currFrameIndex++;
            if (_currFrameIndex >= Animation.FramesCount)
            {
                this.Reset();
                this.OnAnimationComplete?.Invoke(this);
                if (!this.Owner.Enabled)
                    return;
            }
        }

        public async ValueTask Render(GameContext game, Blazorex.IRenderContext context)
        {
            if (null == Animation || !this.Owner.Enabled)
                return;

            context.Save();

            context.Translate(_transform.World.Position.X + (MirrorVertically ? Animation.FrameSize.Width : 0f), _transform.World.Position.Y);
            context.Rotate(_transform.World.Rotation);
            context.Scale(_transform.World.Scale.X * (MirrorVertically ? -1f : 1f), _transform.World.Scale.Y);

            context.DrawImage(Animation.ImageRef,
                _currFramePosX, _currFramePosY,
                Animation.FrameSize.Width, Animation.FrameSize.Height,
                Animation.HalfFrameSize.Width, Animation.HalfFrameSize.Height,
                -Animation.FrameSize.Width, -Animation.FrameSize.Height);

            context.Restore();
        }

        public AnimationCollection.Animation Animation
        {
            get => _animation;
            set
            {
                if (_animation == value)
                    return;
                _currFramePosX = _currFramePosY = 0;
                _animation = value;
            }
        }

        public void Reset()
        {
            _currFramePosX = 0;
            _currFramePosY = 0;
            _currFrameIndex = 0;
        }

        public event OnAnimationCompleteHandler OnAnimationComplete;
        public delegate void OnAnimationCompleteHandler(AnimatedSpriteRenderComponent renderer);

        public bool MirrorVertically { get; set; }
        public int LayerIndex { get; set; }
        public bool Hidden { get; set; }
    }
}