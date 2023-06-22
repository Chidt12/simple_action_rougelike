using Runtime.Definition;
using Runtime.Gameplay.CollisionDetection;
using Runtime.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    [RequireComponent(typeof(CircleCollisionShapeBody))]
    public class AutoStableGun : MonoBehaviour
    {
        [SerializeField] private CircleCollisionShapeBody _collision;
        [SerializeField] private Transform _flipPivotTransform;
        [SerializeField] private Transform _rotateTransform;
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private AnimatorHolder _animatorHolder;
        [SerializeField] private Transform _rangeObject;

        private const float ROTATE_SPEED = 1200;
        private List<IEntityData> _targets;
        private IEntityData _currentTarget;
        private EntityType[] _targetTypes;
        private bool _isShooting;
        private float _currentTime;
        private float _cooldown;

        private Action<Transform, Vector2> _onShootingAction; // Spawn point and direction

        public void Init(float detectRange, float cooldown, EntityType[] targetTypes, Action<Transform, Vector2> onShootingAction)
        {
            _collision.Init(detectRange);
            _targetTypes = targetTypes;
            _cooldown = cooldown;
            _targets = new List<IEntityData>();
            _onShootingAction = onShootingAction;
            _collision.OnCollisionEvent = OnCollision;
            _rangeObject.localScale = new Vector2(detectRange * 2, detectRange * 2);
        }

        private void OnCollision(CollisionResult result, ICollisionBody collisionBody)
        {
            var collider = collisionBody.Collider;
            if (collider)
            {
                var entityHolder = collider.GetComponent<EntityHolder>();
                if (_targetTypes.Contains(entityHolder.EntityData.EntityType))
                {
                    if (result.collisionType == CollisionType.Enter)
                    {
                        if (!entityHolder.EntityData.IsDead)
                        {
                            _targets.Add(entityHolder.EntityData);
                            UpdateCurrentTarget();
                        }
                    }
                    else if (result.collisionType == CollisionType.Exit)
                    {
                        if (_targets.Contains(entityHolder.EntityData))
                        {
                            _targets.Remove(entityHolder.EntityData);
                            UpdateCurrentTarget();
                        }
                    }
                }
            }
        }

        private void UpdateCurrentTarget()
        {
            if(_currentTarget.IsDead || !_targets.Contains(_currentTarget))
            {
                _currentTarget = _targets.FirstOrDefault(x => !x.IsDead);
            }
        }

        public void OnUpdate(float deltaTime)
        {
            if(!_isShooting)
                _currentTime += Time.deltaTime;

            if (_currentTarget != null)
            {
                var direction = _currentTarget.Position - (Vector2)_rotateTransform.position;
                var toRotation = direction.ToQuaternion(0);
                _rotateTransform.rotation = Quaternion.RotateTowards(_rotateTransform.rotation, toRotation, ROTATE_SPEED * Time.deltaTime);
                var degree = Quaternion.Angle(_rotateTransform.rotation, Quaternion.identity);
                if (degree > 90 || degree < -90)
                    _flipPivotTransform.localScale = new Vector3(1, -1, 1);
                else
                    _flipPivotTransform.localScale = new Vector3(1, 1, 1);
            }
        }

        public bool CanShooting()
        {
            return _currentTime >= _cooldown && !_isShooting && _currentTarget != null && !_currentTarget.IsDead;
        }

        public void Shooting()
        {
            _currentTime = 0;
            _isShooting = true;
            _animatorHolder.SetEvents(OnAnimationTriggeredPoint, () => _isShooting = false);
        }

        #region UNITY EVENT

        public void OnAnimationTriggeredPoint()
        {
            _onShootingAction?.Invoke(_spawnPoint, _rotateTransform.up);
        }

        #endregion UNITY EVENT
    }
}