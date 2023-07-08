using Runtime.Definition;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public class FlyProjectileStrategyData : ProjectileStrategyData
    {
        #region Members

        public float moveDistance;
        public float moveSpeed;

        #endregion Members

        #region Class Methods

        public FlyProjectileStrategyData(float moveDistance, float moveSpeed, Action<ProjectileCallbackData> callbackAction)
            : base(callbackAction)
        {
            this.moveDistance = moveDistance;
            this.moveSpeed = moveSpeed;
        }

        #endregion Class Methods
    }

    public abstract class FlyProjectileStrategy<T> : ProjectileStrategy<T> where T : FlyProjectileStrategyData
    {
        #region Properties

        protected Vector2 currentDirection;
        protected Vector2 originalPosition;

        #endregion Properties

        #region Class Methods

        public override void Init(ProjectileStrategyData projectileStrategyData, Projectile controllerProjectile, Vector2 direction, Vector2 originalPosition, Vector2 targetPosition, IEntityData targetData = null)
        {
            base.Init(projectileStrategyData, controllerProjectile, direction, originalPosition, targetPosition, targetData);
            this.originalPosition = originalPosition;
            this.currentDirection = direction;
        }

        public override void Update()
        {
            if (Vector2.SqrMagnitude(originalPosition - controllerProjectile.CenterPosition) > strategyData.moveDistance * strategyData.moveDistance)
                ReachedTheLifeDistance();
        }

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
                else CollidedDeathTarget();
            }
            else CollidedObstacle();
        }

        protected abstract void ReachedTheLifeDistance();

        protected abstract void CollidedObstacle();
        protected abstract void CollidedDeathTarget();

        protected virtual void HitTarget(IEntityData target, Vector2 hitPoint, Vector2 hitDirection) { }

        #endregion Class Methods
    }
}