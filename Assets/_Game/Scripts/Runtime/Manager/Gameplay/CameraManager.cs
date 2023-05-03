using Cinemachine;
using Runtime.Core.Message;
using Runtime.Core.Singleton;
using Runtime.Message;
using UnityEngine;
using ZBase.Foundation.PubSub;

namespace Runtime.Manager.Gameplay
{
    public class CameraManager : MonoSingleton<CameraManager>
    {
        [SerializeField]
        private CinemachineVirtualCamera _virtualCamera;
        private ISubscription _heroSpawnedSubScription;

        protected override void Awake()
        {
            base.Awake();
            SimpleMessenger.Publish(new UpdateCameraMessage(Camera.main));
            _heroSpawnedSubScription = SimpleMessenger.Subscribe<HeroSpawnedMessage>(OnHeroSpawned);
        }

        private void OnDestroy()
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