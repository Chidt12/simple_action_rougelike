using Runtime.Gameplay.EntitySystem;
using System;
using UnityEngine;

namespace Runtime.Gameplay
{
    [RequireComponent(typeof(Collider2D))]
    public class DamageBox : MonoBehaviour
    {
        [SerializeField] private Collider2D _collider;

        private Action<IEntityData> _onTriggeredEntered;
        private Action<IEntityData> _onTriggeredExit;
        private bool _isInited;

        public Vector2 CenterBoxPosition => _collider.bounds.center;

        private void OnDisable()
        {
            _isInited = false;
        }

        private void OnEnable()
        {
            _collider.enabled = false;
        }

        public void StartDamage(Action<IEntityData> onTriggeredEntered, Action<IEntityData> onTriggeredExit = null)
        {
            _isInited = true;
            _onTriggeredEntered = onTriggeredEntered;
            _onTriggeredExit = onTriggeredExit;
            _collider.enabled = true;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!_isInited)
                return;

            var entityHolder = collision.GetComponent<IEntityHolder>();
            if (entityHolder != null && entityHolder.EntityData != null && !entityHolder.EntityData.IsDead)
            {
                _onTriggeredEntered?.Invoke(entityHolder.EntityData);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (!_isInited)
                return;

            var entityHolder = collision.GetComponent<IEntityHolder>();
            if (entityHolder != null && entityHolder.EntityData != null && !entityHolder.EntityData.IsDead)
            {
                _onTriggeredExit?.Invoke(entityHolder.EntityData);
            }
        }
    }
}