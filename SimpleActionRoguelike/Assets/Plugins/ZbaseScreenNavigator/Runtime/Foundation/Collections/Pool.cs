using System.Collections.Generic;
using UnityEngine;

namespace ZBase.UnityScreenNavigator.Foundation
{
    internal class Pool<T> where T : new()
    {
        public static Pool<T> s_shared = new();

        public static Pool<T> Shared => s_shared;

        private readonly Queue<T> _queue = new();

        /// <seealso href="https://docs.unity3d.com/Manual/DomainReloading.html"/>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Init()
        {
            s_shared = new();
        }

        public T Rent()
            => _queue.Count == 0 ? new T() : _queue.Dequeue();

        public void Return(T instance)
            => _queue.Enqueue(instance);
    }
}
