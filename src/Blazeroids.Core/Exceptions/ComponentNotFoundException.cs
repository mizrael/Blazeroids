using System;
using Blazeroids.Core.Components;

namespace Blazeroids.Core.Exceptions
{
    public class ComponentNotFoundException<TC> : Exception where TC : Component
    {
        public ComponentNotFoundException() : base($"{typeof(TC).Name} not found on owner")
        {
        }
    }
}