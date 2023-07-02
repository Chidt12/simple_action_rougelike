using Cysharp.Threading.Tasks;
using Runtime.Definition;
using Runtime.Manager.Gameplay;
using System;
using System.Threading;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem 
{
    public class EntityDashBehavior : EntityBehavior<IEntityControlData, IEntityStatData>, IDisposable
    {
        [SerializeField] private float _dashTime;
        [SerializeField] private float _speed;
        [SerializeField] private float _dashCooldown;
        [SerializeField] private ParticleSystem _dashEffect;

        private EntityStatWithCurrentValue _dashStat;

        private IEntityControlData _controlData;
        private CancellationTokenSource _dashCooldownCancellationTokenSource;
        private CancellationTokenSource _cancellationTokenSource;

        protected override UniTask<bool> BuildDataAsync(IEntityControlData data, IEntityStatData statData)
        {
            if(data != null && statData != null)
            {
                _controlData = data;

                if(statData.TryGetStat(StatType.DashNumber, out var dash))
                {
                    _dashStat = dash as EntityStatWithCurrentValue;

                    _controlData.PlayActionEvent += OnDash;
                    _cancellationTokenSource = new();
                    _dashCooldownCancellationTokenSource = new();
                    _dashEffect.Stop();
                    return UniTask.FromResult(true);
                }
            }
            return UniTask.FromResult(false);
        }

        private void OnDash(ActionInputType actionInputType)
        {
            if(actionInputType == ActionInputType.Dash)
            {
                if(_controlData.CanDash && _dashStat.CurrentValue >= 1)
                {
                    PresentDashAsync(_cancellationTokenSource.Token).Forget();
                    _dashStat.DecreaseCurrentValue(1);
                    StartDashCooldownAsync(_dashCooldownCancellationTokenSource.Token).Forget();
                }
            }
        }

        private async UniTaskVoid StartDashCooldownAsync(CancellationToken token)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(_dashCooldown), cancellationToken: token);
            _dashStat.IncreaseCurrenValue(1);
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
                _controlData.ForceUpdatePosition.Invoke(currentFaceDirection.normalized * _speed * Time.deltaTime + _controlData.Position, false);
                currentTime -= Time.deltaTime;
                await UniTask.WaitForFixedUpdate(cancellationToken: token);
                if (!MapManager.Instance.IsWalkable(_controlData.Position))
                {
                    break;
                }
            }

            _controlData.IsDashing = false;
            _controlData.ReactionChangedEvent.Invoke(EntityReactionType.JustFinishDashing);
            _dashEffect.Stop();
        }

        public void Dispose()
        {
            _cancellationTokenSource?.Cancel();
            _dashCooldownCancellationTokenSource?.Cancel();
        }
    }

}