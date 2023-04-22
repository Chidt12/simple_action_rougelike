using System;
using System.Collections.Generic;

namespace Runtime.SceneLoading
{
    [Serializable]
    public class SceneLoadingInfo
    {
        #region Members

        public string sceneName = "Scene Name";
        public float sceneLoadingMinTime;
        public float fadeInDelay;
        public float fadeInSpeed;
        public float fadeOutDelay;
        public float fadeOutSpeed;
        public List<string> sceneLoadingTips;

        #endregion Members

        #region Properties

        public string LoadedSceneName { get; private set; }

        #endregion Properties

        #region Class Methods

        public void UpdateLoadedSceneName(string loadedSceneName)
            => LoadedSceneName = loadedSceneName;

        #endregion Class Methods
    }
}