using Runtime.Core.Message;
using Runtime.Definition;
using Runtime.Gameplay.EntitySystem;
using Runtime.Message;
using System;
using System.Linq;
using UnityEngine;
using ZBase.Foundation.PubSub;

namespace Runtime.Manager.Gameplay
{
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class MapGate : MonoBehaviour
    {
        [Serializable]
        public class MapGateGraphic
        {
            public GameplayRoomType gateType;
            public Sprite icon;
        }

        private const float WAITING_TIME = 0.3f;

        [SerializeField] private GameObject _mainGraphic;
        [SerializeField] private GameObject _shadow;
        [SerializeField] private GameObject _guideGraphic;
        [SerializeField] private SpriteRenderer _renderer;
        [SerializeField] private Collider2D _collider2D;
        [SerializeField] private MapGateGraphic[] _graphics;

        private ISubscription _subscription;
        private MapGateGraphic _currentGraphic;
        private bool _heroEntered;
        private bool _isSubmitted;

        private void Awake()
        {
            _collider2D.enabled = false;
            _guideGraphic.SetActive(false);
            _subscription = SimpleMessenger.Subscribe<InputKeyPressMessage>(OnKeyPress);
        }

        private void OnDestroy()
        {
            _subscription.Dispose();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            var entityHolder = collision.GetComponent<IEntityHolder>();
            if (entityHolder != null && entityHolder.EntityData.EntityType == EntityType.Hero)
            {
                _guideGraphic.SetActive(true);
                _heroEntered = true;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            var entityHolder = collision.GetComponent<IEntityHolder>();
            if (entityHolder != null && entityHolder.EntityData.EntityType == EntityType.Hero)
            {
                _heroEntered = false;
                _guideGraphic.SetActive(false);
            }
        }

        private void OnKeyPress(InputKeyPressMessage message)
        {
            if(message.KeyPressType == KeyPressType.Interact && _heroEntered)
            {
                if (_isSubmitted)
                    return;

                _isSubmitted = true;
                SimpleMessenger.Publish(new SendToGameplayMessage(SendToGameplayType.GoNextStage, _currentGraphic.gateType));
            }
        }

        public void SetUp(GameplayRoomType gateType)
        {
            _isSubmitted = false;
            _collider2D.enabled = false;
            foreach (var graphic in _graphics)
            {
                if(graphic.gateType == gateType)
                {
                    _currentGraphic = graphic;
                    _renderer.sprite = graphic.icon;
                }
            }

            _mainGraphic.SetActive(false);
            _shadow.SetActive(false);
        }

        public void OpenGate()
        {
            _mainGraphic.SetActive(true);
            _shadow.SetActive(true);
            _collider2D.enabled = true;
        }
    }

}