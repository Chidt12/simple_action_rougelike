using Runtime.Core.Message;
using Runtime.Extensions;
using Runtime.Message;
using System;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public class FlyForwardProjectileStrategyData : FlyProjectileStrategyData
    {
        public FlyForwardProjectileStrategyData(float moveDistance, float moveSpeed, Action<ProjectileCallbackData> callbackAction)
            : base(moveDistance, moveSpeed, callbackAction) { }
    }

    public class FlyForwardProjectileStrategy : FlyForwardProjectileStrategy<FlyForwardProjectileStrategyData> { }

    public abstract class FlyForwardProjectileStrategy<T> : FlyProjectileStrategy<T> where T : FlyProjectileStrategyData
    {
        #region Class Methods

        public override void Start()
        {
            base.Start();
            controllerProjectile.UpdateRotation(currentDirection.ToQuaternion());
        }

        public override void Update()
        {
            controllerProjectile.UpdatePositionBySpeed(strategyData.moveSpeed, currentDirection);
            base.Update();
        }

        protected override void HitTarget(IEntityData target, Vector2 hitPoint, Vector2 hitDirection)
        {
            strategyData.callbackAction?.Invoke(new ProjectileCallbackData(hitDirection, hitDirection, target));
            controllerProjectile.CompleteStrategy(true);
        }

        #endregion Class Methods
    }
}