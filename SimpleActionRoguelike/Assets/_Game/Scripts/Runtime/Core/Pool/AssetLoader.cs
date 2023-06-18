using Cysharp.Threading.Tasks;
using Runtime.Core.Singleton;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Runtime.Core.Pool
{
    public class AssetLoader : PersistentMonoSingleton<AssetLoader> 
    {
        private Dictionary<string, Sprite> _spriteAssetsDictionary;

        protected override void Awake()
        {
            base.Awake();
            _spriteAssetsDictionary = new Dictionary<string, Sprite>();
        }

        public static async UniTask<Sprite> LoadSprite(string assetId, CancellationToken cancellationToken)
        {
            Sprite assetSprite = null;
            if (!Instance._spriteAssetsDictionary.ContainsKey(assetId))
            {
                assetSprite = await Addressables.LoadAssetAsync<Sprite>(assetId).WithCancellation(cancellationToken);
                if (!Instance._spriteAssetsDictionary.ContainsKey(assetId))
                    Instance._spriteAssetsDictionary.Add(assetId, assetSprite);
            }
            else assetSprite = Instance._spriteAssetsDictionary[assetId];
            return assetSprite;
        }
    }
}
