using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace Runtime.SceneLoading
{
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField]
        private float _sceneLoadingProgressSmoothSpeed = 3.0f;
        [SerializeField]
        private float _mainLoadScale = 1.0f;
        [SerializeField]
        private float _progressBarRoundValue = 0;
        [SerializeField]
        private SceneLoaderUI _sceneLoaderScreen;

        private bool _isLoadingScene;
        private bool _hasFinishedLoading;
        private float _sceneLoadingProgressSmoothValue;
        private SceneLoadingInfo _sceneLoadingInfo;

        public float DeltaTime => Time.unscaledDeltaTime;
        public SceneLoadingInfo SceneLoadingInfo => _sceneLoadingInfo;


        private void Awake() => Init();
        private void OnEnable() => SceneManager.activeSceneChanged += OnSceneChanged;
        private void OnDisable() => SceneManager.activeSceneChanged -= OnSceneChanged;

        public async UniTask LoadLevelAsync(SceneLoadingInfo sceneLoadingInfo)
        {
            if (_isLoadingScene)
                return;

            _isLoadingScene = true;
            _sceneLoadingInfo = sceneLoadingInfo;
            _sceneLoaderScreen.UpdatesceneLoadingInfo(_sceneLoadingInfo);
            await StartSceneLoadingAsyncOperation(_sceneLoadingInfo.LoadedSceneName);
        }

        private void Init()
        {
            _isLoadingScene = false;
            _hasFinishedLoading = false;
            _sceneLoadingProgressSmoothValue = 0.0f;
            _sceneLoaderScreen.Init(this);
        }

        private void OnSceneChanged(Scene oldScene, Scene newScene)
            => ResetWhenSceneChanged();

        private IEnumerator StartSceneLoadingAsyncOperation(string sceneName)
        {
            yield return _sceneLoaderScreen.PrepareLoadingScene();

            var sceneLoadingAsyncOperation = Addressables.LoadSceneAsync(sceneName);
            var currentLoadingTime = 0.0f;

            while (!sceneLoadingAsyncOperation.IsDone)
            {
                var status = sceneLoadingAsyncOperation.GetDownloadStatus();
                float completeProgress = Mathf.Clamp01(status.Percent + (1 - Constants.Constant.SCENE_LOADING_STOP_PROGRESS_VALUE));
                _sceneLoadingProgressSmoothValue = Mathf.Lerp(_sceneLoadingProgressSmoothValue, completeProgress, DeltaTime * _sceneLoadingProgressSmoothSpeed);
                float progressBarValue = _progressBarRoundValue > 0 ? (Mathf.Round(_sceneLoadingProgressSmoothValue / _progressBarRoundValue) * _progressBarRoundValue)
                                                                    : _sceneLoadingProgressSmoothValue;
                progressBarValue = progressBarValue * _mainLoadScale;
                _sceneLoaderScreen.UpdateLoadProgress(progressBarValue, _sceneLoadingProgressSmoothValue * _mainLoadScale);

                currentLoadingTime += DeltaTime;
                if (completeProgress >= Constants.Constant.SCENE_LOADING_STOP_PROGRESS_VALUE)
                {
                    if (currentLoadingTime > _sceneLoadingInfo.sceneLoadingMinTime)
                    {
                        if (!_hasFinishedLoading)
                        {
                            _hasFinishedLoading = true;
                            _sceneLoaderScreen.TransitionToNewScene();
                        }
                    }
                    else yield return null;
                }

                yield return null;
            }

            _isLoadingScene = false;
        }

        private void ResetWhenSceneChanged()
        {
            _hasFinishedLoading = false;
            _sceneLoadingProgressSmoothValue = 0.0f;
            _sceneLoaderScreen.ResetWhenSceneChanged();
        }
    }
}