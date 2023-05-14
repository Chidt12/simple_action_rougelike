using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public interface IAttackStrategy : IDisposable
    {
        #region Interface Methods

        void Init(WeaponModel weaponModel, IEntityStatData entityData, Transform creatorTransform);
        void InitEventProxy(IEntityTriggerActionEventProxy triggerActionEventProxy);
        bool CheckCanAttack();
        bool CheckCanSpecialAttack();
        UniTask OperateAttack();
        UniTask OperateSpecialAttack();
        void Cancel();

        #endregion Interface Methods
    }
}
