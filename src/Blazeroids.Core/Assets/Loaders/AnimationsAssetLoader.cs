using System;
using System.Drawing;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Blazeroids.Core.Assets.Loaders
{
    public class AnimationsAssetLoader : IAssetLoader<AnimationCollection>
    {
        private readonly HttpClient _httpClient;

        public AnimationsAssetLoader(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async ValueTask<AnimationCollection> Load(AssetMeta meta)
        {
            var dto = await _httpClient.GetFromJsonAsync<AnimationsFile>(meta.Path);

            var asset = dto.ToAsset();
            
            return asset;
        }

        internal class AnimationsFile
        {
            public string name { get; set; }
            public AnimationData[] animations { get; set; }

            public AnimationCollection ToAsset()
            {
                var asset = new AnimationCollection(this.name);

                if (this.animations is not null)
                {
                    foreach (var animDto in this.animations)
                    {
                        var elementRef = new ElementReference(Guid.NewGuid().ToString());
                        var anim = animDto.ToAsset(elementRef, asset);
                    }
                }
                return asset;
            }

            internal class AnimationData
            {
                public AnimationCollection.Animation ToAsset(ElementReference elementRef, AnimationCollection asset) =>
                    new AnimationCollection.Animation(this.name, this.fps,
                    this.frameSize.ToSize(), this.framesCount,
                    elementRef, this.imageData,
                    new Size(this.imageMeta.width, this.imageMeta.height),
                    asset);

                public string name { get; set; }
                public int fps { get; set; }
                public int framesCount { get; set; }
                public FrameSize frameSize { get; set; }
                public ImageMeta imageMeta { get; set; }
                public string imageData { get; set; }

                internal class ImageMeta
                {
                    public int width { get; set; }
                    public int height { get; set; }
                    public string type { get; set; }
                }

                internal class FrameSize
                {
                    public int width { get; set; }
                    public int height { get; set; }
                    public Size ToSize() => new Size(width, height);
                }
            }
        }
    }
}