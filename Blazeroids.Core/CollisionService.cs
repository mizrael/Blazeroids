using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using Blazeroids.Core.Components;

namespace Blazeroids.Core
{
    internal class CollisionBucket
    {   
        private readonly HashSet<BoundingBoxComponent> _colliders = new();

        public CollisionBucket(Rectangle bounds)
        {
            Bounds = bounds;
        }

        public void Add(BoundingBoxComponent bbox) => _colliders.Add(bbox);
        
        public Rectangle Bounds { get; }
    }
    
    public class CollisionService : IGameService
    {
        private readonly GameContext _game;
        
        private CollisionBucket[,] _buckets;
        private readonly Size _bucketSize;
        
        public CollisionService(GameContext game, Size bucketSize)
        {
            _game = game;
            _bucketSize = bucketSize;
            _game.Display.OnSizeChanged += BuildBuckets;
        }

        private void BuildBuckets()
        {
            var rows = _game.Display.Size.Height / _bucketSize.Height;
            var cols = _game.Display.Size.Width / _bucketSize.Width;
            _buckets = new CollisionBucket[rows, cols];

            for (int row = 0; row < rows; row++)
            for (int col = 0; col < cols; col++)
            {
                var bounds = new Rectangle(
                    col * _bucketSize.Width,
                    row * _bucketSize.Height,
                    _bucketSize.Width, 
                    _bucketSize.Height);
                _buckets[row, col] = new CollisionBucket(bounds);
            }
            
            var colliders = FindAllColliders();
            foreach (var collider in colliders)
            {
                AddToBuckets(collider);
            }
        }

        private void AddToBuckets(BoundingBoxComponent collider)
        {
            var rows = _buckets.GetLength(0);
            var cols = _buckets.GetLength(1);
            var startX = (int) (cols * ((float) collider.Bounds.Left / _game.Display.Size.Width));
            var startY = (int) (rows * ((float) collider.Bounds.Top / _game.Display.Size.Height));

            var endX = (int) (cols * ((float) collider.Bounds.Right / _game.Display.Size.Width));
            var endY = (int) (rows * ((float) collider.Bounds.Bottom / _game.Display.Size.Height));

            for (int row = startY; row <= endY; row++)
            for (int col = startX; col <= endX; col++)
            {
                if (row < 0 || row >= rows)
                    continue;
                if (col < 0 || col >= cols)
                    continue;
                
                if (_buckets[row, col].Bounds.IntersectsWith(collider.Bounds))
                    _buckets[row, col].Add(collider);
            }
        }

        private IEnumerable<BoundingBoxComponent> FindAllColliders()
        {
            var scenegraph = _game.GetService<SceneGraph>();
            var colliders = new List<BoundingBoxComponent>();

            FindAllColliders(scenegraph.Root, colliders);
            
            return colliders;
        }

        private void FindAllColliders(GameObject node, IList<BoundingBoxComponent> colliders)
        {
            if (node is null)
                return;
            
            if(node.Components.TryGet<BoundingBoxComponent>(out var bbox))
                colliders.Add(bbox);
            
            if(node.Children is not null)
                foreach(var child in node.Children)
                    FindAllColliders(child, colliders);
        }

        public ValueTask Step()
        {
            if(null == _buckets)
                BuildBuckets();
            return ValueTask.CompletedTask;
        }
    }
}