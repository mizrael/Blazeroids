using System.Collections.Generic;
using Blazor.Extensions;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Blazeroids.Core.Web.Components
{
    public abstract class CanvasManagerBase : ComponentBase
    {
        protected readonly Dictionary<string, CanvasOptions> _names = new();
        protected readonly Dictionary<string, BECanvasComponent> _canvases = new();

        public ValueTask<BECanvasComponent> CreateCanvas(string name)
            => CreateCanvas(name, CanvasOptions.Defaults);
            
        public async ValueTask<BECanvasComponent> CreateCanvas(string name, CanvasOptions options)
        {
            _names.Add(name, options);

            this.StateHasChanged();

            await this.OnCanvasAdded.InvokeAsync();

            return _canvases[name];
        }

        [Parameter]
        public EventCallback OnCanvasAdded { get; set; }
    }

    public struct CanvasOptions
    {
        public bool Hidden { get; set; }
        public readonly static CanvasOptions Defaults = new CanvasOptions()
        {
            Hidden = false
        };
    }
}