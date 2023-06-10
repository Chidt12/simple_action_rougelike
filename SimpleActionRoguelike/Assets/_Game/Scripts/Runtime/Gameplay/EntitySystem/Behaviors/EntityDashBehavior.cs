using Cysharp.Threading.Tasks;
using Runtime.Definition;
using System;
using System.Threading;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem 
{
    public class EntityDashBehavior : EntityBehavior<IEntityControlData>, IDisposable
    {
        [SerializeField] private float _dashTime;
        [SerializeField] private float _speed;
        [SerializeField] private float _delayDashTime;
        [SerializeField] private float _invincibleTime;
        [SerializeField] private ParticleSystem _dashEffect;

        private bool _isDelayingDash;
        private IEntityControlData _controlData;
        private CancellationTokenSource _dashCancellationTokenSource;
        private CancellationTokenSource _cancellationTokenSource;

        protected override UniTask<bool> BuildDataAsync(IEntityControlData data)
        {
            if(data != null)
            {
                _isDelayingDash = false;
                _controlData = data;
                _controlData.PlayActionEvent += OnDash;
                _controlData.ReactionChangedEvent += OnReactionChanged;
                _cancellationTokenSource = new();
                _dashEffect.Stop();
                return UniTask.FromResult(true);
            }
            else
            {
                return UniTask.FromResult(false);
            }
        }

        private void OnReactionChanged(EntityReactionType reactionType)
        {
            if(reactionType == EntityReactionType.JustPlayAttack || reactionType == EntityReactionType.JustPlaySkill)
            {
                if (_controlData.IsDashing)
                {
                    _dashCancellationTokenSource?.Cancel();
                    _controlData.IsDashing = false;
                    _controlData.ReactionChangedEvent.Invoke(EntityReactionType.JustFinishDashing);
                    _dashEffect.Stop();
                    StartDelayDashAsync().Forget();
                }
            }
        }

        private void OnDash(ActionInputType actionInputType)
        {
            if(actionInputType == ActionInputType.Dash)
            {
                if(_controlData.CanDash && !_isDelayingDash)
                {
                    _dashCancellationTokenSource = new();
                    PresentDashAsync(_dashCancellationTokenSource.Token).Forget();
                }
            }
        }

        private async UniTaskVoid PresentDashAsync(CancellationToken token)
        {
            _controlData.IsDashing = true;
            _controlData.ReactionChangedEvent.Invoke(EntityReactionType.JustDashing);
            _dashEffect.Play();

            var currentFaceDirection = _controlData.LastMoveDirection;
            var currentTime = _dashTime;

            while(currentTime >= 0)
            {
                _controlData.ForceUpdatePosition.Invoke(currentFaceDirection.normalized * _speed * Time.deltaTime + _controlData.Position);
                currentTime -= Time.deltaTime;
                await UniTask.WaitForFixedUpdate(cancellationToken: token);
            }

            _controlData.IsDashing = false;
            _controlData.ReactionChangedEvent.Invoke(EntityReactionType.JustFinishDashing);
            _dashEffect.Stop();
            StartDelayDashAsync().Forget();
        }

        private async UniTaskVoid StartDelayDashAsync()
        {
            _isDelayingDash = true;
            _controlData.IsInvincible = true;
            await UniTask.Delay(TimeSpan.FromSeconds(_invincibleTime), cancellationToken: _cancellationTokenSource.Token);
            _controlData.IsInvincible = false;
            await UniTask.Delay(TimeSpan.FromSeconds(_delayDashTime), cancellationToken: _cancellationTokenSource.Token);
            _isDelayingDash = false;
        }

        public void Dispose()
        {
            _cancellationTokenSource?.Cancel();
            _dashCancellationTokenSource?.Cancel();
        }
    }

}