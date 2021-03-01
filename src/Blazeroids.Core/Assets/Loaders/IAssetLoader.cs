using System.Threading.Tasks;

namespace Blazeroids.Core.Assets.Loaders
{
    public interface IAssetLoader<TA> where TA : IAsset
    {
        ValueTask<TA> Load(AssetMeta meta);
    }
}