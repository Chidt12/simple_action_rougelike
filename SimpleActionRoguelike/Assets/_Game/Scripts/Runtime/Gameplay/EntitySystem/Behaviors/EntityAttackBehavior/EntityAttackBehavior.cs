using Cysharp.Threading.Tasks;
using Runtime.Definition;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    [DisallowMultipleComponent]
    public class EntityAttackBehavior : EntityBehavior<IEntityControlData, IEntityWeaponData, IEntityStatData, IEntityStatusData>
    {
        private IAttackStrategy _attackStrategy;
        private IEntityControlData _controlData;
        private IEntityStatusData _statusData;
        private IEntityWeaponData _weaponData;
        private IEntityStatData _statData;

        protected async override UniTask<bool> BuildDataAsync(IEntityControlData data, IEntityWeaponData weaponData, IEntityStatData statData, IEntityStatusData statusData)
        {
            if(data == null || weaponData == null || statData == null)
                return false;

            _statData = statData;
            _controlData = data;
            _controlData.PlayActionEvent += OnTriggerAttack;

            _weaponData = weaponData;
            _weaponData.UpdateWeaponModel += OnUpdateWeapon;

            await UpdateWeapon();

            if (statusData != null)
            {
                _statusData = statusData;
                _statusData.UpdateCurrentStatus += OnUpdateCurrentStatus;
            }

            return true;
        }

        private void OnUpdateWeapon() => UpdateWeapon();

        private UniTask UpdateWeapon()
        {
            _attackStrategy?.Cancel();
            _attackStrategy = GetComponentInChildren<IAttackStrategy>();
            _attackStrategy.Init(_weaponData.WeaponModel, _statData, transform);
            _attackStrategy.InitEventProxy(GetComponent<IEntityTriggerActionEventProxy>());
            _weaponData.IsAttacking = false;

            return UniTask.CompletedTask;
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
            _controlData.ReactionChangedEvent.Invoke(EntityReactionType.JustPlayAttack);

            await _attackStrategy.OperateAttack();

            _weaponData.IsAttacking = false;
            _controlData.ReactionChangedEvent.Invoke(EntityReactionType.JustFinishedAttack);
        }

        private async UniTaskVoid RunSpecialAttackAsync()
        {
            _weaponData.IsAttacking = true;
            _controlData.ReactionChangedEvent.Invoke(EntityReactionType.JustPlayAttack);

            await _attackStrategy.OperateSpecialAttack();

            _weaponData.IsAttacking = true;
            _controlData.ReactionChangedEvent.Invoke(EntityReactionType.JustFinishedAttack);
        }

        public void Disable()
        {
            _attackStrategy.Dispose();
        }
    }
}
