using Runtime.Definition;
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
        public float damageBonus;
        public DamageFactor[] damageFactors;

        #endregion Members

        #region Class Methods

        public FlyProjectileStrategyData(DamageSource damageSource, float moveDistance, float moveSpeed,
                                        float damageBonus = 0, DamageFactor[] damageFactors = null)
            : base(damageSource)
        {
            this.moveDistance = moveDistance;
            this.moveSpeed = moveSpeed;
            this.damageBonus = damageBonus;
            this.damageFactors = damageFactors;
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

        //public override void Init(ProjectileStrategyData projectileStrategyData, Projectile controllerProjectile, Vector2 direction, Vector2 originalPosition, IEntityPositionData targetData = null)
        //{
        //    base.Init(projectileStrategyData, controllerProjectile, direction, originalPosition, targetData);
        //    this.originalPosition = originalPosition;
        //    this.currentDirection = direction;
        //}

        public override void Update()
        {
            if (Vector2.SqrMagnitude(originalPosition - controllerProjectile.CenterPosition) > strategyData.moveDistance * strategyData.moveDistance)
                controllerProjectile.CompleteStrategy(true);
        }

        public override void Collide(Collider2D collider)
        {
            var entity = collider.GetComponent<EntityHolder>();
            if (entity != null)
            {
                //var interactable = entity.GetBehavior<IInteractable>();
                //if (interactable != null && !interactable.Model.IsDead)
                //{
                //    if (controllerProjectile.Creator.EntityType.CanCauseDamage(interactable.Model.EntityType))
                //    {
                //        var hitPoint = collider.ClosestPoint(controllerProjectile.CenterPosition);
                //        var hitDirection = controllerProjectile.Direction;
                //        HitTarget(interactable, hitPoint, hitDirection);
                //    }
                //}
                //else controllerProjectile.CompleteStrategy(true);
            }
            else controllerProjectile.CompleteStrategy(true);
        }

        //protected virtual void HitTarget(IInteractable target, Vector2 hitPoint, Vector2 hitDirection) { }

        #endregion Class Methods
    }
}