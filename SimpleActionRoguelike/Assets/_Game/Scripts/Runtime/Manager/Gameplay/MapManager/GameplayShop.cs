using Runtime.Core.Message;
using Runtime.Definition;
using Runtime.Gameplay.EntitySystem;
using Runtime.Message;
using UnityEngine;
using ZBase.Foundation.PubSub;

namespace Runtime.Gameplay.UI
{
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class GameplayShop : MonoBehaviour
    {
        [SerializeField] private GameObject _guideGraphic;
        [SerializeField] private SpriteRenderer _sprite;
        [SerializeField] private Sprite _activeSprite;
        [SerializeField] private Sprite _normalSprite;

        private ISubscription _subscription;
        private bool _heroEntered;
        private bool _isSubmitted;

        private void Awake()
        {
            _guideGraphic.SetActive(false);
            _subscription = SimpleMessenger.Subscribe<InputKeyPressMessage>(OnKeyPress);
        }

        private void OnEnable()
        {
            _sprite.sprite = _normalSprite;
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
                _sprite.sprite = _activeSprite;
                _guideGraphic.SetActive(true);
                _heroEntered = true;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            var entityHolder = collision.GetComponent<IEntityHolder>();
            if (entityHolder != null && entityHolder.EntityData.EntityType == EntityType.Hero)
            {
                _sprite.sprite = _normalSprite;
                _heroEntered = false;
                _guideGraphic.SetActive(false);
            }
        }

        private void OnKeyPress(InputKeyPressMessage message)
        {
            if (message.KeyPressType == KeyPressType.Interact && _heroEntered)
            {
                SimpleMessenger.Publish(new SendToGameplayMessage(SendToGameplayType.BuyShop));
            }
        }
    }
}