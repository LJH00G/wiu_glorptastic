using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utility
{
    public class ComponentCache
    {
        readonly Dictionary<Type, Component> cache = new();

        /// <summary>
        /// calls <see cref="Component.TryGetComponent{T}(out T)"/> from <paramref name="source"/> if called with <typeparamref name="T"/> for the first time, cache and return the result
        /// <br/>
        /// all subsequent calls with the same <typeparamref name="T"/> will return cached component instead
        /// </summary>
        public T Get<T>(Component source)
            where T : Component
        {
            Type type = typeof(T);
            if (cache.TryGetValue(type, out Component cpnt))
                return (T)cpnt;
            source.TryGetComponent(out T cpnt_t);
            cache[type] = cpnt_t;
            return cpnt_t;
        }

        /// <summary>
        /// refreshes cache of <typeparamref name="T"/> by calling <see cref="Component.TryGetComponent{T}(out T)"/> from <paramref name="source"/>, returns the result
        /// </summary>
        public T RefreshCashe<T>(Component source)
            where T : Component
        {
            source.TryGetComponent(out T cpnt_t);
            cache[typeof(T)] = cpnt_t;
            return cpnt_t;
        }
    }
}