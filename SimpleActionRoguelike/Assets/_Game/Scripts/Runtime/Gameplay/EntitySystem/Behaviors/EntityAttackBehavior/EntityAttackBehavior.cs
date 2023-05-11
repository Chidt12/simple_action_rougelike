using Cysharp.Threading.Tasks;
using Runtime.Definition;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    [DisallowMultipleComponent]
    public class EntityAttackBehavior : EntityBehavior<IEntityControlData, IEntityWeaponData, IEntityStatData, IEntityStatusData>
    {
        private IAttackStrategy _attackStrategy;
        private IEntityControlData controlData;
        private IEntityStatusData _statusData;
        private IEntityWeaponData _weaponData;

        protected override UniTask<bool> BuildDataAsync(IEntityControlData data, IEntityWeaponData weaponData, IEntityStatData statData, IEntityStatusData statusData)
        {
            controlData = data;
            _weaponData = weaponData;
            controlData.PlayActionEvent += OnTriggerAttack;
            _attackStrategy = AttackStrategyFactory.GetAttackStrategy(_weaponData.WeaponModel.WeaponType);
            _attackStrategy.Init(weaponData.WeaponModel, statData, transform);
            _attackStrategy.InitEventProxy(GetComponent<IEntityTriggerActionEventProxy>());

            if (statusData != null)
            {
                _statusData = statusData;
                _statusData.UpdateCurrentStatus += OnUpdateCurrentStatus;
            }

            return UniTask.FromResult(true);
        }

        private void OnUpdateCurrentStatus()
        {
            if (_statusData.CurrentState.IsInHardCCStatus())
            {
                _attackStrategy.Cancel();
                _weaponData.IsAttacking = false;
            }
        }

        private void OnTriggerAttack(ActionInputType inputType)
        {
            if (inputType == ActionInputType.Attack)
            {
                if (_weaponData.CheckCanAttack() && _attackStrategy.CheckCanAttack())
                {
                    RunAttackAsync().Forget();
                }
            }
            else if (inputType == ActionInputType.Attack1)
            {
                if (_weaponData.CheckCanAttack() && _attackStrategy.CheckCanSpecialAttack())
                {
                    RunSpecialAttackAsync().Forget();
                }
            }
        }

        private async UniTaskVoid RunAttackAsync()
        {
            _weaponData.IsAttacking = true;
            controlData.ReactionChangedEvent.Invoke(EntityReactionType.JustPlayAttack);

            await _attackStrategy.OperateAttack();

            _weaponData.IsAttacking = false;
            controlData.ReactionChangedEvent.Invoke(EntityReactionType.JustFinishedAttack);
        }

        private async UniTaskVoid RunSpecialAttackAsync()
        {
            _weaponData.IsAttacking = true;
            controlData.ReactionChangedEvent.Invoke(EntityReactionType.JustPlayAttack);

            await _attackStrategy.OperateSpecialAttack();

            _weaponData.IsAttacking = true;
            controlData.ReactionChangedEvent.Invoke(EntityReactionType.JustFinishedAttack);
        }

        public void Disable()
        {
            _attackStrategy.Dispose();
        }
    }
}
