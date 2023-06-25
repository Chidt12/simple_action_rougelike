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

        private EntityType _detectEntityType;

        private void OnEnable()
        {
            _collisionHolder.gameObject.SetActive(false);
        }

        public void Init(EntityType detectEntityType, float width, float height, Action<IEntityModifiedStatData> entityEntered, Action<IEntityModifiedStatData> entityExited)
        {
            _detectEntityType = detectEntityType;

            _visualTransform.localScale = new Vector2(width, height);
            _collisionHolder.gameObject.SetActive(true);
            _collisionHolder.OnCollisionEntered = OnCollisionEntered;
            _collisionHolder.OnCollisionExited = OnCollisionExited;

        }

        private void OnCollisionEntered(Collider2D obj)
        {
            var
        }

        private void OnCollisionExited(Collider2D obj)
        {
            throw new NotImplementedException();
        }
    }
}