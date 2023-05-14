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
        public override void Collide(Collider2D collider)
        {
            var entityHolder = collider.GetComponent<EntityHolder>();
            if (entityHolder != null)
            {
                if (!entityHolder.EntityData.IsDead)
                {
                    if (controllerProjectile.Creator.EntityType.CanCauseDamage(entityHolder.EntityData.EntityType))
                    {
                        var hitPoint = collider.ClosestPoint(controllerProjectile.CenterPosition);
                        var hitDirection = controllerProjectile.Direction;
                        HitTarget(entityHolder.EntityData, hitPoint, hitDirection);
                    }
                }
                else
                {
                    if (strategyData.obstacleOnly)
                        controllerProjectile.CompleteStrategy(true);
                }
            }
        }

        protected override void HitTarget(IEntityData target, Vector2 hitPoint, Vector2 hitDirection)
        {
            strategyData.callbackAction?.Invoke(new ProjectileCallbackData(hitDirection, hitDirection, target));
            if (strategyData.obstacleOnly)
                controllerProjectile.CompleteStrategy(true);
        }
    }
}