using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Runtime.Helper;
using Object = UnityEngine.Object;

namespace Runtime.Core.AssetLoader
{
    public static class GameDataLoader
    {
        private static readonly AssetLoader s_loader;

        private static readonly Dictionary<Type, ScriptableObject> cachedData;

        static GameDataLoader()
        {
            s_loader = ZBase.Foundation.Singletons.Singleton.Of<AssetLoader>();
            cachedData = new Dictionary<Type, ScriptableObject>();
        }

        public static void Init()
        {
            // do nothing, as instance will be created in constructor
        }

        public static async UniTask<GameObject> Instantiate(string resourcePath, Transform parent)
        {
            return await s_loader.Instantiate(resourcePath, parent);
        }

        public static async UniTask<T> Load<T>(string resourcePath, bool loadAsync = true) where T : Object
        {
            return await s_loader.Load<T>(resourcePath, loadAsync);
        }

        public static async UniTask<T> Load<T>() where T : ScriptableObject
        {
            if (cachedData.TryGetValue(typeof(T), out ScriptableObject data))
            {
                return data as T;
            }

            return await Preload<T>();
        }

        public static async UniTask<T> Preload<T>() where T : ScriptableObject
        {
            T data = await s_loader.Load<T>();
            cachedData.Replace(typeof(T), data);

            return data;
        }

        public static T GetCsv<T>() where T : ScriptableObject
        {
            if (cachedData.TryGetValue(typeof(T), out ScriptableObject data))
            {
                return data as T;
            }

            return null;
        }
    }
}
