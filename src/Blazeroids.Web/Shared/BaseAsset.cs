using Blazeroids.Core.Assets;
using Microsoft.AspNetCore.Components;

namespace Blazeroids.Web.Shared
{
    public abstract class BaseAsset<TItem> : ComponentBase
        where TItem : IAsset
    {
        [Parameter]
        public TItem Source { get; set; }

        [Parameter]
        public EventCallback OnLoaded { get; set; }
    }
}
