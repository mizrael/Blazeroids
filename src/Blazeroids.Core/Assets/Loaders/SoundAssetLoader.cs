using System.Threading.Tasks;

namespace Blazeroids.Core.Assets.Loaders
{
    public class SoundAssetLoader : IAssetLoader<Sound>
    {
        public ValueTask<Sound> Load(AssetMeta meta)
        {
            var type = meta.Properties["type"].ToString();
            var name = meta.Properties["name"].ToString();
            var sound = new Sound(meta.Path, name, type);
            return ValueTask.FromResult(sound);
        }
    }
}