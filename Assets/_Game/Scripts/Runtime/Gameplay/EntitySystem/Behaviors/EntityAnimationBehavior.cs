using Cysharp.Threading.Tasks;
using Runtime.Definition;
using System;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public enum UpdateFaceRightType
    {
        MoveDirection,
        FaceDirection,
    }

    [DisallowMultipleComponent]
    public class EntityAnimationBehavior : EntityBehavior<IEntityControlData, IEntityStatusData>, IDisposeEntityBehavior, IEntityTriggerActionEventProxy
    {
        [SerializeField] private Transform _flipTransform;
        [SerializeField] private UpdateFaceRightType _updateFaceRightType;
        private IEntityControlData _controlData;
        private IEntityStatusData _statusData;
        private IEntityAnimation[] _entityAnimations;
        private bool _canUpdateAnimation;
        private AnimationType _currentAnimationType;
        private bool _isPaused;

        public void Dispose()
        {
            foreach (var item in _entityAnimations)
                item.Dispose();
        }

        protected override UniTask<bool> BuildDataAsync(IEntityControlData data, IEntityStatusData statusData)
        {
            if (data == null)
                return UniTask.FromResult(false);

            _isPaused = false;

            _entityAnimations = GetComponentsInChildren<IEntityAnimation>(true);
            if (_entityAnimations.Length == 0)
                return UniTask.FromResult(false);

            _controlData = data;
            _controlData.MovementChangedEvent += UpdateCurrentAnimation;

            if (_updateFaceRightType == UpdateFaceRightType.FaceDirection)
                _controlData.DirectionChangedEvent += OnFaceRightUpdateByFaceDirection;
            else if (_updateFaceRightType == UpdateFaceRightType.MoveDirection)
                _controlData.MovementUpdatedValueEvent += OnFaceRightUpdateByMoveDirection;

            _controlData.ReactionChangedEvent += OnReactionChanged;

            foreach (var item in _entityAnimations)
                item.Init(_controlData);

            _canUpdateAnimation = true;
            UpdateCurrentAnimation();

            if(statusData != null)
            {
                _statusData = statusData;
                _statusData.UpdateCurrentStatus += OnUpdateCurrentStatus;
            }

            return UniTask.FromResult(true);
        }

        private void OnUpdateCurrentStatus()
        {
            if(_statusData.CurrentState.IsInAnimationLockedStatus())
            {
                if(!_isPaused)
                {
                    _isPaused = true;
                    foreach (var animation in _entityAnimations)
                        animation.Pause();
                }
            }
            else
            {
                if (_isPaused)
                {
                    _isPaused = false;
                    foreach (var animation in _entityAnimations)
                        animation.Continue();
                }
            }

            if (_statusData.CurrentState.IsInMovementLockedStatus())
                TriggerEvent(AnimationType.Idle);
            else
                UpdateCurrentAnimation();
        }

        private void OnReactionChanged(EntityReactionType reactionType)
        {
            if(reactionType == EntityReactionType.JustFinishedAttack || reactionType == EntityReactionType.JustFinishedUseSkill)
            {
                _canUpdateAnimation = true;
                UpdateCurrentAnimation();
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

        private void UpdateCurrentAnimation()
        {
            if (!_canUpdateAnimation)
                return;

            if (_controlData.IsMoving)
            {
                if(_currentAnimationType != AnimationType.Run)
                    TriggerEvent(AnimationType.Run);
            }    
            else
            {
                if (_currentAnimationType != AnimationType.Idle)
                    TriggerEvent(AnimationType.Idle);
            }    
        }

        private void OnFaceRightUpdateByFaceDirection()
        {
            if (_controlData.FaceDirection.x > 0)
                _flipTransform.localScale = new Vector2(1, 1);
            else
                _flipTransform.localScale = new Vector2(-1, 1);
        }

        private void OnFaceRightUpdateByMoveDirection()
        {
            var moveVector = _controlData.MoveDirection;
            if (moveVector.x > 0)
                _flipTransform.localScale = new Vector2(1, 1);
            else
                _flipTransform.localScale = new Vector2(-1, 1);
        }

        public void TriggerEvent(AnimationType animationType, Action<SetStateData> stateAction = null, Action<SetStateData> endAction = null, bool isRefresh = false)
        {
            bool assignedEvent = false;
            _currentAnimationType = animationType;
            foreach (var entityAnimation in _entityAnimations)
            {
                entityAnimation.Play(animationType);
                if ((stateAction != null || endAction != null) && entityAnimation.IsMainPart(animationType) && !assignedEvent)
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