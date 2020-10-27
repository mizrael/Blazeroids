using System;
using System.Drawing;
using System.IO;
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

        public async ValueTask<Sprite> Load(string path)
        {
            var bytes = await _httpClient.GetByteArrayAsync(path);
            await using var stream = new MemoryStream(bytes);
            using var image = await SixLabors.ImageSharp.Image.LoadAsync(stream);
            var size = new Size(image.Width, image.Height);

            var elementRef = new ElementReference(Guid.NewGuid().ToString());
            return new Sprite(path, elementRef, size, bytes, ImageFormatUtils.FromPath(path));
        }
    }
}