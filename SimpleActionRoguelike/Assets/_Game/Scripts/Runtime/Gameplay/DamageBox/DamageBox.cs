using Runtime.Gameplay.EntitySystem;
using System;
using UnityEngine;

namespace Runtime.Gameplay
{
    [RequireComponent(typeof(Collider2D))]
    public class DamageBox : MonoBehaviour
    {
        private Action<IEntityData> _onTriggeredEntered;
        private Action<IEntityData> _onTriggeredExit;
        private bool _isInited;

        private void OnDisable()
        {
            _isInited = false;
        }

        public void Init(Action<IEntityData> onTriggeredEntered, Action<IEntityData> onTriggeredExit = null)
        {
            _isInited = true;
            _onTriggeredEntered = onTriggeredEntered;
            _onTriggeredExit = onTriggeredExit;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!_isInited)
                return;

            var entityHolder = collision.GetComponent<EntityHolder>();
            if (entityHolder && entityHolder.EntityData != null && !entityHolder.EntityData.IsDead)
            {
                _onTriggeredEntered?.Invoke(entityHolder.EntityData);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (!_isInited)
                return;

            var entityHolder = collision.GetComponent<EntityHolder>();
            if (entityHolder && entityHolder.EntityData != null && !entityHolder.EntityData.IsDead)
            {
                _onTriggeredExit?.Invoke(entityHolder.EntityData);
            }
        }
    }
}