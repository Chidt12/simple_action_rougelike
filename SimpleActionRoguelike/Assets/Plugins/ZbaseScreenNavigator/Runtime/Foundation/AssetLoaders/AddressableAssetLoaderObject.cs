#if USN_USE_ADDRESSABLES
using UnityEngine;

namespace ZBase.UnityScreenNavigator.Foundation.AssetLoaders
{
    [CreateAssetMenu(fileName = "AddressableAssetLoader", menuName = "Screen Navigator/Loaders/Addressables Asset Loader")]
    public sealed class AddressableAssetLoaderObject : AssetLoaderObject, IAssetLoader
    {
        [SerializeField]
        private bool _suppressErrorLogOnRelease;

        private readonly AddressableAssetLoader _loader = new();

        public override AssetLoadHandle<T> Load<T>(string key)
        {
            return _loader.Load<T>(key);
        }

        public override AssetLoadHandle<T> LoadAsync<T>(string key)
        {
            return _loader.LoadAsync<T>(key);
        }

        public override void Release(AssetLoadHandleId handleId)
        {
            _loader.SuppressErrorLogOnRelease = _suppressErrorLogOnRelease;
            _loader.Release(handleId);
        }
    }
}
#endif