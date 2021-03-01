using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blazeroids.Core.Assets
{
    public interface IAssetsResolver
    {
        ValueTask<TA> Load<TA>(AssetMeta meta) where TA : IAsset;
        TA Get<TA>(string path) where TA : class, IAsset;
    }

    public record AssetMeta(string Path, string Type, IDictionary<string, object> Properties);
}