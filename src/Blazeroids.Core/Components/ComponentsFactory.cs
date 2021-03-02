using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace Blazeroids.Core.Components
{
    internal class ComponentsFactory
    {
        private ConcurrentDictionary<Type, ConstructorInfo> _cTorsByType;

        private ComponentsFactory()
        {
            _cTorsByType = new ConcurrentDictionary<Type, ConstructorInfo>();
        }

        private static readonly Lazy<ComponentsFactory> _instance = new Lazy<ComponentsFactory>(new ComponentsFactory());
        public static ComponentsFactory Instance => _instance.Value;

        public TC Create<TC>(GameObject owner) where TC : Component
        {
            var ctor = GetCtor<TC>();

            return ctor.Invoke(new[] {owner}) as TC;
        }

        private ConstructorInfo GetCtor<TC>() where TC : Component
        {
            var type = typeof(TC);

            if (!_cTorsByType.ContainsKey(type))
            {
                var ctor = type.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, null,
                    new[] { typeof(GameObject) }, null);
                _cTorsByType.AddOrUpdate(type, ctor, (t, c) => ctor);
            }

            return _cTorsByType[type];
        }
    }
}