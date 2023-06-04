using Cinemachine;
using Runtime.Core.Message;
using Runtime.Message;
using System;
using UnityEngine;
using ZBase.Foundation.PubSub;

namespace Runtime.Manager.Gameplay
{
    [Serializable]
    public class CameraManager : MonoBehaviour
    {
        [SerializeField]
        private CinemachineVirtualCamera _virtualCamera;

        private ISubscription _heroSpawnedSubScription;

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
        }
    }
}