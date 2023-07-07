using System;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public class IdleThroughProjectileStrategyData : ProjectileStrategyData
    {
        public bool obstacleOnly;

        public IdleThroughProjectileStrategyData(bool obstacleOnly, Action<ProjectileCallbackData> callbackAction) : base(callbackAction)
        {
            this.obstacleOnly = obstacleOnly;
        }
    }

    public class IdleThroughProjectileStrategy : ProjectileStrategy<IdleThroughProjectileStrategyData>
    {
        public override void Collide(Collider2D collider)
        {
            var entityHolder = collider.GetComponent<IEntityHolder>();
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
                    if(strategyData.obstacleOnly)
                        Complete(false, true);
                }
            }
        }

        protected virtual void HitTarget(IEntityData target, Vector2 hitPoint, Vector2 hitDirection)
        {
            strategyData.callbackAction?.Invoke(new ProjectileCallbackData(hitDirection, hitDirection, target));
            if (strategyData.obstacleOnly)
                Complete(false, true);
        }
    }
}
