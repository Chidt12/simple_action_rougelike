using Runtime.Definition;
using Runtime.Gameplay.CollisionDetection;
using System;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public class EffectArea : MonoBehaviour
    {
        [SerializeField] private UnityCollisionHolder _collisionHolder;
        [SerializeField] private Transform _visualTransform;
        [SerializeField] private GameObject _activeEffect;

        private EntityType _detectEntityType;
        private Action<IEntityModifiedStatData> _entityEntered;
        private Action<IEntityModifiedStatData> _entityExited;

        private void OnEnable()
        {
            _collisionHolder.gameObject.SetActive(false);
        }

        public void Init(EntityType detectEntityType, float width, float height, Action<IEntityModifiedStatData> entityEntered, Action<IEntityModifiedStatData> entityExited)
        {
            ToggleEnableVisual(false);
            _detectEntityType = detectEntityType;
            _visualTransform.localScale = new Vector2(width, height);
            _collisionHolder.gameObject.SetActive(true);

            _entityEntered = entityEntered;
            _entityExited = entityExited;
            _collisionHolder.OnCollisionEntered = OnCollisionEntered;
            _collisionHolder.OnCollisionExited = OnCollisionExited;
        }

        public void ToggleEnableVisual(bool value)
        {
            _activeEffect.SetActive(value);
        }

        private void OnCollisionEntered(Collider2D obj)
        {
            var entityHolder = obj.GetComponent<EntityHolder>();
            if(entityHolder && entityHolder.EntityData.EntityType == _detectEntityType)
            {
                var entityModifiedData = entityHolder.EntityData as IEntityModifiedStatData;
                _entityEntered?.Invoke(entityModifiedData);
            }
        }

        private void OnCollisionExited(Collider2D obj)
        {
            var entityHolder = obj.GetComponent<EntityHolder>();
            if (entityHolder && entityHolder.EntityData.EntityType == _detectEntityType)
            {
                var entityModifiedData = entityHolder.EntityData as IEntityModifiedStatData;
                _entityExited?.Invoke(entityModifiedData);
            }
        }
    }
}