using System;
using Blazeroids.Core;
using Blazeroids.Core.Utils;

namespace Blazeroids.Web.Game.GameObjects
{
    public class Spawner : GameObject
    {
        private readonly Action<GameObject> _onItemSpawn;
        private readonly Pool<GameObject> _pool;

        public Spawner(Func<GameObject> factory, Action<GameObject> onItemSpawn)
        {
            _onItemSpawn = onItemSpawn;
            _pool = new Pool<GameObject>(factory);
        }

        public GameObject Spawn()
        {
            var item = _pool.Get();

            if (item.OnDisabled != this.OnItemDisabled)
                item.OnDisabled += this.OnItemDisabled;
            
            this.AddChild(item);

            _onItemSpawn(item);

            item.Enabled = true;

            return item;
        }

        private void OnItemDisabled(GameObject item)
        {
            _pool.Return(item);
        }
    }
}