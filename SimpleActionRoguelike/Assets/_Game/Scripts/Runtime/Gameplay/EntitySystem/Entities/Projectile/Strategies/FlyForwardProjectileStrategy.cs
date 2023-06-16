using Runtime.Helper;
using System;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public class FlyForwardProjectileStrategyData : FlyProjectileStrategyData
    {
        public FlyForwardProjectileStrategyData(float moveDistance, float moveSpeed, Action<ProjectileCallbackData> callbackAction)
            : base(moveDistance, moveSpeed, callbackAction) { }
    }

    public class FlyForwardProjectileStrategy : FlyForwardProjectileStrategy<FlyForwardProjectileStrategyData>
    {
    }

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
            Complete(false, true);
        }

        protected override void CollidedDeathTarget()
        {
            Complete(false, true);
        }

        protected override void CollidedObstacle()
        {
            Complete(false, true);
        }

        protected override void ReachedTheLifeDistance()
        {
            Complete(false, true);
        }

        #endregion Class Methods
    }
}