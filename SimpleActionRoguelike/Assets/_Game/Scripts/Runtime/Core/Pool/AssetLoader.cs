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
        private Dictionary<string, Material> _materialDictionary;

        protected override void Awake()
        {
            base.Awake();
            _spriteAssetsDictionary = new Dictionary<string, Sprite>();
            _materialDictionary = new Dictionary<string, Material>();
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

        public static async UniTask<Material> LoadMaterial(string assetId, CancellationToken cancellationToken)
        {
            Material material = null;
            if (!Instance._materialDictionary.ContainsKey(assetId))
            {
                material = await Addressables.LoadAssetAsync<Material>(assetId).WithCancellation(cancellationToken);
                if (!Instance._materialDictionary.ContainsKey(assetId))
                    Instance._materialDictionary.Add(assetId, material);
            }
            else
            {
                material = Instance._materialDictionary[assetId];
            }

            return material;
        }
    }
}
