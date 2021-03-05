using System.Threading.Tasks;
using Blazeroids.Core.GameServices;
using Microsoft.JSInterop;

namespace Blazeroids.Web.Game.GameServices
{
    public interface ISoundService : IGameService{
        ValueTask Play(string name, bool loop = false);
    }

    public class SoundService : ISoundService
    {
        private readonly IJSRuntime _jsRuntime;

        public SoundService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public ValueTask Step() => ValueTask.CompletedTask;

        public async ValueTask Play(string name, bool loop = false){
            await _jsRuntime.InvokeAsync<object>("playSound", name, loop);
        }
    }
}