using System;
using System.Drawing;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Blazeroids.Core.Assets.Loaders
{
    public class SpriteAssetLoader : IAssetLoader<Sprite>
    {
        private readonly HttpClient _httpClient;
        
        public SpriteAssetLoader(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async ValueTask<Sprite> Load(AssetMeta meta)
        {
            if (null == meta)
                throw new ArgumentNullException(nameof(meta));
            if (null == meta.Properties)
                throw new ArgumentException("properties missing", nameof(AssetMeta.Properties));
            
            if(!meta.Properties.TryGetValue("width", out var tmp) ||
               !int.TryParse(tmp.ToString(), out var width))
                throw new ArgumentException("invalid width", nameof(AssetMeta.Properties));

            if (!meta.Properties.TryGetValue("height", out tmp) ||
                !int.TryParse(tmp.ToString(), out var height))
                throw new ArgumentException("invalid height", nameof(AssetMeta.Properties));

            var bounds = new Rectangle(0, 0, width, height);

            var elementRef = new ElementReference(Guid.NewGuid().ToString());
            return new Sprite(meta.Path, elementRef, bounds, meta.Path);
        }
    }
}