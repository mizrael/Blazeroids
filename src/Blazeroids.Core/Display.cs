using System;
using System.Drawing;
using Blazeroids.Core.Web.Components;

namespace Blazeroids.Core
{
    public class Display
    {
        public Display(CanvasManagerBase canvasManager)
        {
            CanvasManager = canvasManager ?? throw new ArgumentNullException(nameof(canvasManager));
        }

        private Size _size;
        public Size Size
        {
            get => _size;
            set
            {
                _size = value;
                OnSizeChanged?.Invoke();
            }
        }

        public event OnSizeChangedHandler OnSizeChanged;
        public delegate void OnSizeChangedHandler();

        public CanvasManagerBase CanvasManager { get; }
    }
}