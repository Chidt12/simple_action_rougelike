using Runtime.Core.Message;
using Runtime.Definition;
using Runtime.Gameplay.EntitySystem;
using Runtime.Manager.Gameplay;
using Runtime.Message;
using UnityEngine;

namespace Runtime.Gameplay
{
    [RequireComponent(typeof(Collider2D))]
    public class ShopChestItem : CheckEndStage
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private Collider2D _collider2D;

        private bool _isInited;
        private bool _isTriggered;
        private bool _isAvailableForEndStage;

        public override bool IsAvailableForEndStage => _isAvailableForEndStage;

        private void OnEnable()
        {
            _isAvailableForEndStage = false;
            _isInited = false;
            _isTriggered = false;
            _collider2D.enabled = false;
            _animator.Play("chest_appear", 0, 0);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!_isInited)
                return;

            var entityHolder = collision.GetComponent<IEntityHolder>();
            if (entityHolder != null && entityHolder.EntityData.EntityType == EntityType.Hero)
            {
                if (_isTriggered)
                    return;
                _isTriggered = true;
                _animator.Play("chest_open", 0, 0);
            }
        }

        #region Unity Event

        public void FinishOpenState()
        {
            _isAvailableForEndStage = true;
            SimpleMessenger.Publish(new SendToGameplayMessage(SendToGameplayType.GiveShopItem));
        }

        public void FinishAppearState()
        {
            _isInited = true;
            _collider2D.enabled = true;
        }

        #endregion Unity Event
    }
}