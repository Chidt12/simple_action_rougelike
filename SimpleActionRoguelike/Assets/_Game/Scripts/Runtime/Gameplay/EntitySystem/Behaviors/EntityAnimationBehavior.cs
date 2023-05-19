using Cysharp.Threading.Tasks;
using DG.Tweening;
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

    [DisallowMultipleComponent]
    public class EntityAnimationBehavior : EntityBehavior<IEntityControlData, IEntityStatusData, IEntityStatData>, IDisposeEntityBehavior, IEntityTriggerActionEventProxy
    {
        private static readonly Color s_appearanceHitEffectColor = new Color(1.0f, 1.0f, 1.0f, 0.85f);
        private static readonly Color s_appearanceNormalColor = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        private static readonly int s_showHitEffectColorTimes = 1;
        private static readonly float s_showHitEffectColorDuration = 0.2f;

        [SerializeField] private Transform _flipTransform;
        [SerializeField] private UpdateFaceRightType _updateFaceRightType;
        [SerializeField] private Transform _playDotweenTransform;
        private IEntityControlData _controlData;
        private IEntityStatusData _statusData;
        private IEntityAnimation[] _entityAnimations;
        private bool _canUpdateAnimation;
        private AnimationType _currentAnimationType;
        private bool _isPaused;
        private CancellationTokenSource _cancellationTokenSource;

        public void Dispose()
        {
            foreach (var item in _entityAnimations)
                item.Dispose();
        }

        protected override UniTask<bool> BuildDataAsync(IEntityControlData data, IEntityStatusData statusData, IEntityStatData statData)
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

            if(statData != null)
            {
                ChangeColor(s_appearanceNormalColor);
                statData.HealthStat.OnDamaged += OnDamaged;
            }

            return UniTask.FromResult(true);
        }

        private void OnDamaged(float damagedValue, EffectSource effectSource, EffectProperty effectProperty)
        {
            if(damagedValue > 0)
            {
                if (_playDotweenTransform)
                {
                    var tween = DOTween.Sequence();
                    tween.Append(_playDotweenTransform.DOScale(1.3f,  0f));
                    tween.Append(_playDotweenTransform.DOScale(1f, 0.3f).SetEase(Ease.InOutSine));
                    tween.Play();
                }

                _cancellationTokenSource?.Cancel();
                _cancellationTokenSource = new CancellationTokenSource();
                ShowHitEffectAsync(s_showHitEffectColorTimes, s_showHitEffectColorDuration, _cancellationTokenSource.Token).Forget();
            }
        }

        private async UniTask ShowHitEffectAsync(int showHitEffectColorTimes, float showHitEffectColorDuration, CancellationToken cancellationToken)
        {
            int currentShowHitEffectColorTimes = 0;
            while (currentShowHitEffectColorTimes < showHitEffectColorTimes)
            {
                currentShowHitEffectColorTimes++;
                ChangeColor(s_appearanceHitEffectColor);
                await UniTask.Delay(TimeSpan.FromSeconds(showHitEffectColorDuration), cancellationToken: cancellationToken);
                ChangeColor(s_appearanceNormalColor);
                await UniTask.Delay(TimeSpan.FromSeconds(showHitEffectColorDuration), cancellationToken: cancellationToken);
            }
            ChangeColor(s_appearanceNormalColor);
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

        public void ChangeColor(Color color)
        {
            foreach (var item in _entityAnimations)
                item.ChangeColor(color);
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