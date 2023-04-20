using UnityEngine;
using ZBase.UnityScreenNavigator.Foundation.AssetLoaders;

namespace Runtime.Core.AssetLoader
{
    public sealed class AssetLoaderObject : IAssetLoader
    {
        private readonly AddressableAssetLoader _loader = new();

        public AssetLoadHandle<T> Load<T>(string key) where T : Object
        {
            return this._loader.Load<T>(key);
        }

        public AssetLoadHandle<T> LoadAsync<T>(string key) where T : Object
        {
            return this._loader.LoadAsync<T>(key);
        }

        public void Release(AssetLoadHandleId handle)
        {
            _loader.Release(handle);
        }
    }
}