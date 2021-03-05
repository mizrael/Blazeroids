using System;

namespace Blazeroids.Core.Assets
{
    public class Sound : IAsset
    {
        public Sound(string url, string name, string type)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentNullException(nameof(url));
            this.Url = url;

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            this.Name = name;

            this.Type = string.IsNullOrWhiteSpace(type) ? "audio/wav" : type;
        }

        public string Url { get; }
        public string Name { get; }
        public string Type { get; }
    }
}