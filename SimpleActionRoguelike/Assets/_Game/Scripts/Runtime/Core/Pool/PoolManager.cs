using Cysharp.Threading.Tasks;
using Runtime.Core.Singleton;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using ZBase.Foundation.Pooling.AddressableAssets;

namespace Runtime.Core.Pool
{
    public class PoolManager : MonoSingleton<PoolManager>
    {
        private readonly Dictionary<string, AddressGameObjectPool> _dictionary = new();

        public AddressGameObjectPool GetPool(string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (this._dictionary.TryGetValue(source, out AddressGameObjectPool pool))
            {
                return pool;
            }

            pool = Create(source, this.transform);

            this._dictionary.Add(source, pool);

            return pool;
        }

        public async UniTask<GameObject> Rent(string source, bool isActive = true, CancellationToken token = default)
        {
            AddressGameObjectPool pool = GetPool(source);
            var gameObject = await pool.Rent(token);
            gameObject.transform.SetParent(null);
            gameObject.name = source;
            gameObject.SetActive(isActive);
            return gameObject;
        }

        public void Return(GameObject instance)
        {
            AddressGameObjectPool pool = GetPool(instance.name);
            if (this._dictionary.TryGetValue(instance.name, out pool))
            {
                pool.Return(instance);
            }
            else
            {
                Destroy(instance);
            }
        }

        public void ReleaseInstances(string source, int keep, Action<GameObject> onReleased = null)
        {
            AddressGameObjectPool pool = GetPool(source);

            pool.ReleaseInstances(keep, onReleased);
        }

        public static AddressGameObjectPool Create(string source, Transform parent = null)
        {
            return new AddressGameObjectPool(new AddressGameObjectPrefab
            {
                Source = source,
                Parent = parent ? parent : new GameObject(source).transform
            });
        }
    }
}