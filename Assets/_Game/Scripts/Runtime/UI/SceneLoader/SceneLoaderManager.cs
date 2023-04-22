using Cysharp.Threading.Tasks;
using Runtime.Core.Singleton;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.SceneLoading
{
    [Serializable]
    public struct SceneLoaderData
    {
        #region Members

        public List<SceneLoadingInfo> scenesLoadingInfo;

        #endregion Members
    }

    public class SceneLoaderManager : PersistentMonoSingleton<SceneLoaderManager>
    {
        [SerializeField]
        private SceneLoaderData _sceneLoaderData;

        private Action ScenePreloadAction { get; set; }

        public static async UniTask LoadSceneAsync(string sceneName)
        {
            Instance.ScenePreloadAction?.Invoke();
            Instance.ScenePreloadAction = null;
            var sceneLoader = FindObjectOfType<SceneLoader>();
            var sceneLoadingInfo = Instance.GetSceneLoadingInfo(sceneName);
            sceneLoadingInfo.UpdateLoadedSceneName(sceneName);
            await sceneLoader.LoadLevelAsync(sceneLoadingInfo);
        }


        public static void RegisterScenePreloadedAction(Action scenePreloadAction)
            => Instance.ScenePreloadAction += scenePreloadAction;

        private SceneLoadingInfo GetSceneLoadingInfo(string sceneName)
        {
            foreach (SceneLoadingInfo sceneLoadingInfo in _sceneLoaderData.scenesLoadingInfo)
                if (sceneName.StartsWith(sceneLoadingInfo.sceneName))
                    return sceneLoadingInfo;

            return null;
        }
    }
}