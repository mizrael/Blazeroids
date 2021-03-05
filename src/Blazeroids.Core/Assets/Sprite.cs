using System;
using System.Drawing;
using Microsoft.AspNetCore.Components;

namespace Blazeroids.Core.Assets
{
    public class Sprite : SpriteBase
    {
        public Sprite(string name, ElementReference elementRef, Rectangle bounds, string imagePath) : base(name, elementRef, bounds)
        {
            if (string.IsNullOrWhiteSpace(imagePath))
                throw new ArgumentNullException(nameof(imagePath));
            this.ImagePath = imagePath;
        }

        public string ImagePath { get; }
    }
}