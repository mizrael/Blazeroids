using System.Numerics;

namespace Blazeroids.Core
{
    public class Transform
    {
        public Vector2 Position;

        public Vector2 Scale;
        
        public float Rotation;

        public void Clone(Transform source)
        {
            this.Position = source.Position;
            this.Scale = source.Scale;
            this.Rotation = source.Rotation;
        }

        public void Reset()
        {
            Position = Vector2.Zero;
            Scale = Vector2.One;
            Rotation = 0f;
        }

        public static Transform Identity() => new Transform()
        {
            Position = Vector2.Zero,
            Scale = Vector2.One,
            Rotation = 0f
        };
    }
}