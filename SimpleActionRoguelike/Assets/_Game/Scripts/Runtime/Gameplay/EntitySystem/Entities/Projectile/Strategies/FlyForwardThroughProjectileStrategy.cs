using System;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public class FlyForwardThroughProjecitleStrategyData : FlyForwardProjectileStrategyData
    {
        public bool obstacleOnly;

        public FlyForwardThroughProjecitleStrategyData(bool onlyObstacle, float moveDistance, float moveSpeed, Action<ProjectileCallbackData> callbackAction)
            : base(moveDistance, moveSpeed, callbackAction) 
        {
            this.obstacleOnly = onlyObstacle;
        }
    }

    public class FlyForwardThroughProjectileStrategy : FlyForwardProjectileStrategy<FlyForwardThroughProjecitleStrategyData>
    {
        protected override void CollidedDeathTarget()
        {
            if (strategyData.obstacleOnly)
                controllerProjectile.CompleteStrategy(true);
        }

        protected override void CollidedObstacle()
        {}

        protected override void HitTarget(IEntityData target, Vector2 hitPoint, Vector2 hitDirection)
        {
            strategyData.callbackAction?.Invoke(new ProjectileCallbackData(hitDirection, hitDirection, target));
            if (strategyData.obstacleOnly)
                controllerProjectile.CompleteStrategy(true);
        }
    }
}