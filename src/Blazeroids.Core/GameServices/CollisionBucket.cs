using System.Collections.Generic;
using System.Drawing;
using Blazeroids.Core.Components;

namespace Blazeroids.Core.GameServices
{
    internal class CollisionBucket
    {   
        private readonly HashSet<BoundingBoxComponent> _colliders = new();

        public CollisionBucket(Rectangle bounds)
        {
            Bounds = bounds;
        }
        
        public Rectangle Bounds { get; }

        public void Add(BoundingBoxComponent bbox) => _colliders.Add(bbox);
        
        public void Remove(BoundingBoxComponent bbox) => _colliders.Remove(bbox);

        public void CheckCollisions(BoundingBoxComponent bbox)
        {
            foreach (var collider in _colliders)
            {
                if (collider.Owner == bbox.Owner || 
                   !collider.Owner.Enabled ||
                   !bbox.Bounds.IntersectsWith(collider.Bounds))
                    continue;

                collider.CollideWith(bbox);
                bbox.CollideWith(collider);
            }
        }
    }
}