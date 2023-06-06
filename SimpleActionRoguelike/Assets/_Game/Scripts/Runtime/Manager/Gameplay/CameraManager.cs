using Cinemachine;
using Cysharp.Threading.Tasks;
using Runtime.Core.Message;
using Runtime.Gameplay.EntitySystem;
using Runtime.Message;
using System;
using UnityEngine;
using ZBase.Foundation.PubSub;

namespace Runtime.Manager.Gameplay
{
    [Serializable]
    public class CameraManager : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera _virtualCamera;
        [SerializeField] private float _shakeAmplitude;
        [SerializeField] private float _shakingTime;

        private ISubscription _heroSpawnedSubScription;
        private IEntityStatData _entityStatData;

        public void Init()
        {
            SimpleMessenger.Publish(new UpdateCameraMessage(Camera.main));
            _heroSpawnedSubScription = SimpleMessenger.Subscribe<HeroSpawnedMessage>(OnHeroSpawned);
        }

        public void Dispose()
        {
            _heroSpawnedSubScription.Dispose();
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
                await UniTask.Delay(TimeSpan.FromSeconds(_shakingTime));
                cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0;
            }
        }
    }
}