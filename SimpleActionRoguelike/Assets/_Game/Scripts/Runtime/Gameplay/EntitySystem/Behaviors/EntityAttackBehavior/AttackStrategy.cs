using Cysharp.Threading.Tasks;
using Runtime.Definition;
using System;
using System.Threading;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public abstract class AttackStrategy<T> : IAttackStrategy where T : WeaponModel
    {
        protected IEntityTriggerActionEventProxy triggerActionEventProxy;
        protected T ownerWeaponModel;
        protected IEntityStatData creatorData;

        protected bool isAttackReady;
        protected float attackSpeed;
        protected float attackCooldownTime;
        protected float currentAttackCooldownTime;
        protected CancellationTokenSource attackCooldownCancellationTokenSource;
        protected CancellationTokenSource executeAttackCancellationTokenSource;
        protected CancellationTokenSource executeSpecialAttackCancellationTokenSource;

        public virtual bool CheckCanAttack() => isAttackReady;

        public bool CheckCanSpecialAttack() => true;

        public virtual void Dispose() => Cancel();

        public void Init(WeaponModel weaponModel, IEntityStatData entityData, Transform creatorTransform)
        {
            triggerActionEventProxy = new DummyEntityTriggerActionEventProxy();
            creatorData = entityData;
            ownerWeaponModel = weaponModel as T;

            if (creatorData.TryGetStat(StatType.AttackSpeed, out var attackSpeedStat))
            {
                attackSpeed = attackSpeedStat.TotalValue;
                attackSpeedStat.OnValueChanged += OnStatChanged;
            }

            isAttackReady = true;
            attackCooldownTime = attackSpeed > 0 ? 1 / (weaponModel.AttackSpeedPercent * attackSpeed) : 0;
            currentAttackCooldownTime = 0.0f;
        }

        protected virtual async UniTaskVoid RunAttackCooldownAsync()
        {
            attackCooldownCancellationTokenSource = new CancellationTokenSource();
            await UniTask.Delay(TimeSpan.FromSeconds(attackCooldownTime), cancellationToken: attackCooldownCancellationTokenSource.Token);
            FinishAttackCooldown();
        }

        private void OnStatChanged(float updatedValue)
        {
            var newAttackCooldownTime = 1 / updatedValue;
            attackCooldownTime = newAttackCooldownTime;
            if (newAttackCooldownTime <= currentAttackCooldownTime)
            {
                attackCooldownCancellationTokenSource?.Cancel();
                FinishAttackCooldown();
            }
        }

        protected virtual void FinishAttackCooldown()
        {
            isAttackReady = true;
            currentAttackCooldownTime = 0.0f;
        }

        public void InitEventProxy(IEntityTriggerActionEventProxy triggerActionEventProxy)
        {
            if(triggerActionEventProxy != null)
                this.triggerActionEventProxy = triggerActionEventProxy;
        }

        public virtual async UniTask OperateAttack()
        {
            isAttackReady = false;
            attackCooldownCancellationTokenSource?.Cancel();
            executeAttackCancellationTokenSource?.Cancel();
            executeAttackCancellationTokenSource = new CancellationTokenSource();
            await TriggerAttack(executeAttackCancellationTokenSource.Token);
            RunAttackCooldownAsync().Forget();
        }

        public virtual async UniTask OperateSpecialAttack()
        {
            executeSpecialAttackCancellationTokenSource?.Cancel();
            executeSpecialAttackCancellationTokenSource = new CancellationTokenSource();
            await TriggerSpecialAttack(executeSpecialAttackCancellationTokenSource.Token);
        }

        protected abstract UniTask TriggerAttack(CancellationToken cancellationToken);
        protected abstract UniTask TriggerSpecialAttack(CancellationToken cancellationToken);

        public virtual void Cancel()
        {
            executeAttackCancellationTokenSource?.Cancel();
            executeSpecialAttackCancellationTokenSource?.Cancel();
            attackCooldownCancellationTokenSource?.Cancel();
            isAttackReady = true;
        }
    }
}