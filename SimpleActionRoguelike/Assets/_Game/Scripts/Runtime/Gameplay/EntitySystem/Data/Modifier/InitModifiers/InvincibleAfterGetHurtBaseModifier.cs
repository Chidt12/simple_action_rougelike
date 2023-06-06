using Cysharp.Threading.Tasks;
using Runtime.Manager.Gameplay;
using System;
using System.Threading;

namespace Runtime.Gameplay.EntitySystem
{
    public class InvincibleAfterGetHurtBaseModifier : IDamageModifier, IDisposable
    {
        private bool _isInInvincibleState;
        private float _invincibleTime = 0.5f;
        private CancellationTokenSource _cancellationTokenSource;

        public int Priority => 1000;

        public InvincibleAfterGetHurtBaseModifier()
        {
            _isInInvincibleState = false;
            _cancellationTokenSource = new();
            GameplayManager.Instance.MessageCenter.AddDamageModifier(this);
        }

        public void Dispose()
        {
            _cancellationTokenSource?.Cancel();
        }

        public float Damage(float value, EffectSource damageSource, EffectProperty damageProperty, IEntityData creator)
        {
            if (_isInInvincibleState)
                return 0;

            if(value > 0)
            {
                _isInInvincibleState = true;
                StartCountTimeAsync(_cancellationTokenSource.Token).Forget();
                return value;
            }

            return value;
        }

        private async UniTaskVoid StartCountTimeAsync(CancellationToken cancellationToken)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(_invincibleTime), cancellationToken: cancellationToken);
            _isInInvincibleState = false;
        }
    }
}