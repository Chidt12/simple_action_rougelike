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
        private Dictionary<string, AudioClip> _audioClipDictionary;

        protected override void Awake()
        {
            base.Awake();
            _audioClipDictionary = new Dictionary<string, AudioClip>();
            _spriteAssetsDictionary = new Dictionary<string, Sprite>();
            _materialDictionary = new Dictionary<string, Material>();
        }

        public static async UniTask<AudioClip> LoadAudioClip(string assetId)
        {
            AudioClip audioClip = null;
            if (!Instance._audioClipDictionary.ContainsKey(assetId))
            {
                audioClip = await Addressables.LoadAssetAsync<AudioClip>(assetId);
                if (!Instance._audioClipDictionary.ContainsKey(assetId))
                    Instance._audioClipDictionary.Add(assetId, audioClip);
            }
            else audioClip = Instance._audioClipDictionary[assetId];
            return audioClip;
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
