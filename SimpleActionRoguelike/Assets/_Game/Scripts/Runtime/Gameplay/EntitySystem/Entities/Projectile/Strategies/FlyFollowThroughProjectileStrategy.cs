using System;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public class FlyFollowThroughProjectileStrategyData : FlyFollowProjectileStrategyData
    {
        public bool obstacleOnly;

        public FlyFollowThroughProjectileStrategyData(bool onlyObstacle, float offsetDegree, float moveDistance, float moveSpeed, Action<ProjectileCallbackData> callbackAction) 
            : base(offsetDegree, moveDistance, moveSpeed, callbackAction)
        {
            this.obstacleOnly = onlyObstacle;
        }
    }

    public class FlyFollowThroughProjectileStrategy : FlyFollowProjectileStrategy<FlyFollowThroughProjectileStrategyData>
    {
        protected override void CollidedDeathTarget()
        {
            if (strategyData.obstacleOnly)
                Complete(false, true);
        }

        protected override void CollidedObstacle()
        {
        }

        protected override void HitTarget(IEntityData target, Vector2 hitPoint, Vector2 hitDirection)
        {
            strategyData.callbackAction?.Invoke(new ProjectileCallbackData(hitDirection, hitDirection, target));
            if (strategyData.obstacleOnly)
                Complete(false, true);
        }
    }
}