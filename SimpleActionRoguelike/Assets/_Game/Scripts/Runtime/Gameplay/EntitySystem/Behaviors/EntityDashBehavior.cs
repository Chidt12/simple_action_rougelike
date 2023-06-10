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
        [SerializeField] private ParticleSystem _dashEffect;

        private IEntityControlData _controlData;
        private CancellationTokenSource _dashCancellationTokenSource;
        private CancellationTokenSource _cancellationTokenSource;

        protected override UniTask<bool> BuildDataAsync(IEntityControlData data)
        {
            if(data != null)
            {
                _controlData = data;
                _controlData.PlayActionEvent += OnDash;
                _cancellationTokenSource = new();
                _dashEffect.Stop();
                return UniTask.FromResult(true);
            }
            else
            {
                return UniTask.FromResult(false);
            }
        }

        private void OnDash(ActionInputType actionInputType)
        {
            if(actionInputType == ActionInputType.Dash)
            {
                if(_controlData.CanDash)
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
        }

        public void Dispose()
        {
            _cancellationTokenSource?.Cancel();
            _dashCancellationTokenSource?.Cancel();
        }
    }

}