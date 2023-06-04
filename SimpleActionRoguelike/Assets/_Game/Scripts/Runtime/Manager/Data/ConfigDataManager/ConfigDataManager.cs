using Cysharp.Threading.Tasks;
using Runtime.Core.Singleton;
using Runtime.Helper;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Runtime.Manager.Data
{
    public partial class ConfigDataManager : MonoSingleton<ConfigDataManager>
    {
        private Dictionary<string, ScriptableObject> _cachedData;

        #region API Methods

        protected override void Awake()
        {
            base.Awake();
            _cachedData = new();
        }

        private void OnDestroy() => ReleaseAll();

        #endregion API Methods

        #region Class Methods

        public T GetData<T>() where T : ScriptableObject
        {
            if (_cachedData.TryGetValue(typeof(T).ToString(), out ScriptableObject data))
            {
                return data as T;
            }

            return null;
        }

        public T GetData<T>(string assetName) where T : ScriptableObject
        {
            if (_cachedData.TryGetValue(assetName, out ScriptableObject data))
            {
                return data as T;
            }

            return null;
        }

        public async UniTask<T> Load<T>() where T : ScriptableObject
        {
            if (_cachedData.TryGetValue(typeof(T).ToString(), out ScriptableObject data))
            {
                return data as T;
            }

            return await Preload<T>();
        }

        public async UniTask<T> Preload<T>() where T : ScriptableObject
        {
            var resourcePath = typeof(T).ToString();
            T data = await Addressables.LoadAssetAsync<T>(resourcePath);
            _cachedData.Replace(resourcePath, data);
            return data;
        }

        public async UniTask<T> Load<T>(string assetName) where T : ScriptableObject
        {
            if (_cachedData.TryGetValue(assetName, out ScriptableObject data))
            {
                return data as T;
            }

            return await Preload<T>(assetName);
        }

        public async UniTask<T> Preload<T>(string assetName) where T : ScriptableObject
        {
            T data = await Addressables.LoadAssetAsync<T>(assetName);
            _cachedData.Replace(assetName, data);
            return data;
        }

        private void ReleaseAll()
        {
            foreach (var data in _cachedData.Values)
                Addressables.Release(data);
            _cachedData.Clear();
        }

        #endregion Class Methods
    }
}