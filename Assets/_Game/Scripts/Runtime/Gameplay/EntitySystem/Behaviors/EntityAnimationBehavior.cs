using Cysharp.Threading.Tasks;
using Runtime.Definition;
using System;
using System.Threading;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public enum UpdateFaceRightType
    {
        MoveDirection,
        FaceDirection,
    }

    public class EntityAnimationBehavior : EntityBehavior<IEntityControlData>, IDisposeEntityBehavior, IEntityTriggerActionEventProxy
    {
        [SerializeField] private Transform _flipTransform;
        [SerializeField] private UpdateFaceRightType _updateFaceRightType;
        [SerializeField] private bool _originalFaceRight;
        private IEntityControlData _controlData;
        private IEntityAnimation[] _entityAnimations;
        private bool _canUpdateAnimation;

        public void Dispose()
        {
            foreach (var item in _entityAnimations)
                item.Dispose();
        }

        protected override UniTask<bool> BuildDataAsync(IEntityControlData data)
        {
            if (data == null)
                return UniTask.FromResult(false);

            _entityAnimations = GetComponentsInChildren<IEntityAnimation>(true);
            if (_entityAnimations.Length == 0)
                return UniTask.FromResult(false);

            _controlData = data;
            _controlData.MovementChangedEvent += OnMovementChanged;

            if (_updateFaceRightType == UpdateFaceRightType.FaceDirection)
                _controlData.DirectionChangedEvent += OnFaceRightUpdateByFaceDirection;
            else if (_updateFaceRightType == UpdateFaceRightType.MoveDirection)
                _controlData.MovementChangedEvent += OnFaceRightUpdateByMoveDirection;

            _controlData.ReactionChangedEvent += OnReactionChanged;

            foreach (var item in _entityAnimations)
                item.Init(_controlData);

            return UniTask.FromResult(true);
        }

        private void OnReactionChanged(EntityReactionType reactionType)
        {
            if(reactionType == EntityReactionType.JustFinishedAttack || reactionType == EntityReactionType.JustFinishedUseSkill)
            {
                _canUpdateAnimation = true;
                OnMovementChanged();
                if (_updateFaceRightType == UpdateFaceRightType.FaceDirection)
                    OnFaceRightUpdateByFaceDirection();
                else if (_updateFaceRightType == UpdateFaceRightType.MoveDirection)
                    OnFaceRightUpdateByMoveDirection();
            }
            else if (reactionType == EntityReactionType.JustPlayAttack || reactionType == EntityReactionType.JustPlaySkill)
            {
                _canUpdateAnimation = false;
            }
        }

        private void OnMovementChanged()
        {
            if (_controlData.IsMoving)
            {
                PlayerAnimation(AnimationType.Run);
            }    
            else
            {
                PlayerAnimation(AnimationType.Idle);
            }    
        }

        private void OnFaceRightUpdateByFaceDirection()
        {
            if (_controlData.FaceDirection.x > 0)
                _flipTransform.localScale = new Vector2(_originalFaceRight ? 1 : -1, 1);
            else
                _flipTransform.localScale = new Vector2(_originalFaceRight ? -1 : 1, 1);
        }

        private void OnFaceRightUpdateByMoveDirection()
        {
            var moveVector = _controlData.MoveDirection;
            if (moveVector.x > 0)
                _flipTransform.localScale = new Vector2(_originalFaceRight ? 1 : -1, 1);
            else
                _flipTransform.localScale = new Vector2(_originalFaceRight ? -1 : 1, 1);
        }

        private void PlayerAnimation(AnimationType animationType)
        {
            if (!_canUpdateAnimation)
                return;
            foreach (var entityAnimation in _entityAnimations)
                entityAnimation.Play(animationType);
        }

        public void TriggerEvent(AnimationType animationType, CancellationToken cancellationToken, Action<SetStateData> stateAction = null, Action<SetStateData> endAction = null, bool isRefresh = false)
        {
            bool assignedEvent = false;
            foreach (var entityAnimation in _entityAnimations)
            {
                entityAnimation.Play(animationType);
                if (entityAnimation.IsMainPart(animationType) && !assignedEvent)
                {
                    assignedEvent = true;
                    entityAnimation.SetTriggeredEvent(animationType, stateAction, endAction);
                }
            }

            if (!assignedEvent)
            {
                stateAction?.Invoke(new SetStateData(new[] { transform }));
                endAction?.Invoke(new SetStateData(new[] { transform }));
            }
        }
    }
}