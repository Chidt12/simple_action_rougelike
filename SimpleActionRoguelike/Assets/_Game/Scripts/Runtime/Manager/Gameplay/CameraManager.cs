using Cinemachine;
using Cysharp.Threading.Tasks;
using Runtime.Core.Message;
using Runtime.Gameplay.EntitySystem;
using Runtime.Message;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using ZBase.Foundation.PubSub;

namespace Runtime.Manager.Gameplay
{
    [Serializable]
    public class CameraManager : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera _virtualCamera;
        [SerializeField] private Camera _uiCamera;
        [SerializeField] private Camera _gameplayCamera;
        [SerializeField] private float _shakeAmplitude;
        [SerializeField] private float _shakingTime;
        [SerializeField] private Canvas _canvas;

        [Header("=== present end game ===")]
        [SerializeField] private float _endGameOrthorSize = 4;
        [SerializeField] private float _timeZoomEndGame = 3;

        private CancellationTokenSource _cancellationTokenSource;
        private List<ISubscription> _subscriptions;
        private IEntityStatData _entityStatData;

        private void Awake()
        {
            _gameplayCamera.gameObject.SetActive(false);
            _uiCamera.gameObject.SetActive(true);
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }

        public void Init()
        {
            _gameplayCamera.gameObject.SetActive(true);
            _uiCamera.gameObject.SetActive(false);

            CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = _virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0;
            _cancellationTokenSource = new();
            SimpleMessenger.Publish(new UpdateCameraMessage(Camera.main));
            _subscriptions = new();
            _subscriptions.Add(SimpleMessenger.Subscribe<HeroSpawnedMessage>(OnHeroSpawned));
            _subscriptions.Add(SimpleMessenger.Subscribe<PresentEndGameCameraMessage>(OnPresentEndGameAsync));
        }

        public void Dispose()
        {
            _gameplayCamera.gameObject.SetActive(false);
            _uiCamera.gameObject.SetActive(true);
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            foreach (var subscription in _subscriptions)
                subscription.Dispose();

            _subscriptions.Clear();
            _cancellationTokenSource?.Cancel();

            CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = _virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0;
        }

        public void SetConfinder(PolygonCollider2D confinder)
        {
            var confinderExtension = _virtualCamera.GetComponent<CinemachineConfiner2D>();
            confinderExtension.m_BoundingShape2D = confinder;
        }

        private void OnHeroSpawned(HeroSpawnedMessage message)
        {
            _virtualCamera.ForceCameraPosition(message.HeroTransform.position, _virtualCamera.transform.rotation);
            _virtualCamera.Follow = message.HeroTransform;

            var statData = message.EntityData as IEntityStatData;
            if(statData != null)
            {
                _entityStatData = statData;
                statData.HealthStat.OnDamaged += OnDamage;
            }
        }
        
        private void OnPresentEndGameAsync(PresentEndGameCameraMessage message)
        {
            PresentEndStageAsync(message.FocusTarget).Forget();
        }

        private async UniTaskVoid PresentEndStageAsync(Transform target)
        {
            _gameplayCamera.gameObject.SetActive(false);
            _uiCamera.transform.position = new Vector3(EntitiesManager.Instance.HeroData.Position.x, EntitiesManager.Instance.HeroData.Position.y, -10);
            _uiCamera.gameObject.SetActive(true);
            _canvas.renderMode = RenderMode.ScreenSpaceCamera;
            _uiCamera.orthographicSize = _gameplayCamera.orthographicSize; 
            float currentTime = 0;
            while(currentTime <= _timeZoomEndGame)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(Time.unscaledDeltaTime), ignoreTimeScale: true, cancellationToken: _cancellationTokenSource.Token);
                currentTime += Time.unscaledDeltaTime;
                var currentZoomValue = Mathf.Lerp(_gameplayCamera.orthographicSize, _endGameOrthorSize, Mathf.Clamp01(currentTime / _timeZoomEndGame));
                _uiCamera.orthographicSize = currentZoomValue;
            }
        }

        private void OnDamage(float value, EffectSource arg2, EffectProperty arg3)
        {
            StartShakingAsync().Forget();
        }

        private async UniTaskVoid StartShakingAsync()
        {
            if(_entityStatData != null && !_entityStatData.IsDead)
            {
                CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = _virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
                cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = _shakeAmplitude;
                await UniTask.Delay(TimeSpan.FromSeconds(_shakingTime), cancellationToken: _cancellationTokenSource.Token);
                cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0;
            }
        }
    }
}