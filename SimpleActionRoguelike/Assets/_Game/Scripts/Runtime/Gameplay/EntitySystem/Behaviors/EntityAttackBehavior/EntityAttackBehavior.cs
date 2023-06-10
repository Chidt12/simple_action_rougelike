using Cysharp.Threading.Tasks;
using Runtime.Core.Pool;
using Runtime.Definition;
using System.Threading;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{


    [DisallowMultipleComponent]
    public class EntityAttackBehavior : EntityBehavior<IEntityControlData, IEntityWeaponData, IEntityStatData, IEntityStatusData>
    {
        [SerializeField] private Transform _weaponHolderTransform;
        private IAttackStrategy _attackStrategy;
        private IEntityControlData _controlData;
        private IEntityStatusData _statusData;
        private IEntityWeaponData _weaponData;
        private IEntityStatData _statData;
        private CancellationTokenSource _cancellationTokenSource;

        protected async override UniTask<bool> BuildDataAsync(IEntityControlData data, IEntityWeaponData weaponData, IEntityStatData statData, IEntityStatusData statusData)
        {
            if(data == null || weaponData == null || statData == null)
                return false;

            _cancellationTokenSource = new();
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

        private void OnUpdateWeapon() => UpdateWeapon().Forget();

        private async UniTask UpdateWeapon()
        {
            _attackStrategy?.Cancel();
            foreach (Transform weaponTransform in _weaponHolderTransform)
                PoolManager.Instance.Return(weaponTransform.gameObject);

            var weaponGameObject = await PoolManager.Instance.Rent(_weaponData.WeaponModel.WeaponPrefabName, token: _cancellationTokenSource.Token);
            weaponGameObject.transform.SetParent(_weaponHolderTransform);
            weaponGameObject.transform.localPosition = Vector2.zero;

            _attackStrategy = weaponGameObject.GetComponent<IAttackStrategy>();
            _attackStrategy.Init(_weaponData.WeaponModel, _statData, transform);

            var triggerEventProxy = GetComponent<IEntityTriggerActionEventProxy>();
            _attackStrategy.InitEventProxy(triggerEventProxy);
            _weaponData.IsAttacking = false;
            triggerEventProxy.UpdateEntityTriggerAction();
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
            var isDashing = _weaponData.IsDashing;
            _controlData.ReactionChangedEvent.Invoke(EntityReactionType.JustPlayAttack);
            await _attackStrategy.OperateAttack(isDashing);
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
            _cancellationTokenSource?.Cancel();
            _attackStrategy.Dispose();
        }
    }
}
