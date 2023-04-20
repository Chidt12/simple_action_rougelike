using Cysharp.Threading.Tasks;
using Runtime.Helper;
using System;
using System.Collections.Generic;
using UnityEngine;
using ZBase.UnityScreenNavigator.Foundation.AssetLoaders;
using Object = UnityEngine.Object;

namespace Runtime.Core.AssetLoader
{
    public sealed class AssetLoader
    {
        private readonly IAssetLoader _assetLoader;

        private readonly Dictionary<string, AssetLoadHandle> _loadedResourceHandles = new();

        public AssetLoader()
        {
            this._assetLoader = new ResourcesAssetLoader();
        }

        public async UniTask<GameObject> Instantiate(string resourcePath, Transform parent)
        {
            GameObject asset = await Load<GameObject>(resourcePath);

            GameObject instance = Object.Instantiate(asset, parent);

            return instance;
        }

        public async UniTask<T> Load<T>(bool isClone = false) where T : ScriptableObject
        {
            var resourcePath = typeof(T).FullName;

            T asset = await Load<T>(resourcePath);

            return isClone ? Object.Instantiate(asset) : asset;
        }

        public async UniTask<T> Load<T>(string resourcePath, bool loadAsync = true) where T : Object
        {
            if (resourcePath == null)
            {
                throw new ArgumentNullException(nameof(resourcePath));
            }

            AssetLoadHandle<T> assetLoadHandle = null;

            if (this._loadedResourceHandles.TryGetValue(resourcePath, out AssetLoadHandle handle))
            {
                assetLoadHandle = (AssetLoadHandle<T>)handle;

                if (handle.Status == AssetLoadStatus.Success)
                {
                    return assetLoadHandle.Result;
                }
            }

            if (assetLoadHandle == null)
            {
                assetLoadHandle = loadAsync
                    ? this._assetLoader.LoadAsync<T>(resourcePath)
                    : this._assetLoader.Load<T>(resourcePath);
                this._loadedResourceHandles.Replace(resourcePath, assetLoadHandle);
            }

            if (!assetLoadHandle.IsDone)
            {
                await UniTask.WaitUntil(() => assetLoadHandle.IsDone);
            }

            if (assetLoadHandle.Status == AssetLoadStatus.Failed)
            {
                this._loadedResourceHandles.Remove(resourcePath);

                throw assetLoadHandle.OperationException;
            }

            return assetLoadHandle.Result;
        }

        public bool IsPreloaded(string resourcePath)
        {
            if (!this._loadedResourceHandles.TryGetValue(resourcePath, out AssetLoadHandle handle))
            {
                return false;
            }

            return handle.Status == AssetLoadStatus.Success;
        }

        public void ReleaseAssetLoaded(string resourcePath)
        {
            if (!this._loadedResourceHandles.ContainsKey(resourcePath))
            {
                throw new InvalidOperationException($"The resource {resourcePath} is not preloaded.");
            }

            AssetLoadHandle handle = this._loadedResourceHandles[resourcePath];
            this._assetLoader.Release(handle.Id);
            this._loadedResourceHandles.Remove(resourcePath);
        }
    }
}