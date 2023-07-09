using System;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public class FlyZigzagProjectileStrategyData : FlyProjectileStrategyData
    {
        public int numberOfHits;

        public FlyZigzagProjectileStrategyData(int numberOfHits, float moveDistance, float moveSpeed, Action<ProjectileCallbackData> callbackAction) 
            : base(moveDistance, moveSpeed, callbackAction)
        {
            this.numberOfHits = numberOfHits;
        }
    }

    public class FlyZigzagProjectileStrategy : FlyProjectileStrategy<FlyZigzagProjectileStrategyData>
    {
        protected int currentHits;
        protected IEntityData currentTarget;
        protected List<int> hittedEntityUids;

        public override void Init(ProjectileStrategyData projectileStrategyData, Projectile controllerProjectile, Vector2 direction, Vector2 originalPosition, Vector2 targetPosition, IEntityData targetData = null)
        {
            base.Init(projectileStrategyData, controllerProjectile, direction, originalPosition, targetPosition, targetData);
            currentHits = 0;
            hittedEntityUids = new();
            FoundFirstTarget(originalPosition, direction);
        }

        private void FoundFirstTarget(Vector2 original, Vector2 direction)
        {
            var ray = new Ray2D(controllerProjectile.CenterPosition, direction.normalized);
            var allEntities = EntitiesManager.Instance.EnemiesData;

            var shortestDistance = float.MaxValue;
            foreach (var item in allEntities)
            {
                if (!item.IsDead)
                {
                    var distance = Vector3.Cross(ray.direction, item.Position - ray.origin).magnitude;
                    if (shortestDistance > distance)
                    {
                        currentTarget = item;
                        shortestDistance = distance;
                    }
                }
            }

            if (currentTarget == null)
                Complete(false, true);
        }

        private void FoundNeareastTarget()
        {
            var allEnemies = EntitiesManager.Instance.EnemiesData;
            var shortestDistance = float.MaxValue;
            bool foundTarget = false;

            foreach (var item in allEnemies)
            {
                if(!item.IsDead && !hittedEntityUids.Contains(item.EntityUID))
                {
                    var distance = Vector2.Distance(controllerProjectile.CenterPosition, item.Position);
                    if(distance <= strategyData.moveDistance && shortestDistance > distance)
                    {
                        shortestDistance = distance;
                        currentTarget = item;
                        foundTarget = true;
                    }
                }
            }

            if (!foundTarget)
            {
                Complete(false, true);
            }
        }

        public override void Update()
        {
            base.Update();

            if(currentTarget != null && !currentTarget.IsDead)
            {
                var moveToPosition = Vector2.MoveTowards(controllerProjectile.CenterPosition, currentTarget.Position, strategyData.moveSpeed * Time.deltaTime);
                
                controllerProjectile.UpdatePosition(moveToPosition);
            }
            else
            {
                CheckHit();
            }
        }

        private void CheckHit()
        {
            currentHits++;
            if (currentHits >= strategyData.numberOfHits)
            {
                Complete(false, true);
            }
            else
            {
                FoundNeareastTarget();
            }
        }

        protected override void CollidedDeathTarget()
        {
            CheckHit();
        }

        protected override void CollidedObstacle()
        {
        }

        protected override void HitTarget(IEntityData target, Vector2 hitPoint, Vector2 hitDirection)
        {
            hittedEntityUids.Add(target.EntityUID);
            strategyData.callbackAction?.Invoke(new ProjectileCallbackData(hitDirection, hitDirection, target));
            CheckHit();
        }

        protected override void ReachedTheLifeDistance()
        {
        }
    }
}