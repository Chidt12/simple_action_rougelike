using Runtime.Helper;
using System;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public class FlyFollowProjectileStrategyData : FlyProjectileStrategyData
    {
        public float offsetDegree;

        public FlyFollowProjectileStrategyData(float offsetDegree, float moveDistance, float moveSpeed, Action<ProjectileCallbackData> callbackAction)
            : base(moveDistance, moveSpeed, callbackAction) 
        {
            this.offsetDegree = offsetDegree;
        }
    }

    public class FlyFollowProjectileStrategy : FlyFollowProjectileStrategy<FlyFollowProjectileStrategyData>
    {
    }

    public class FlyFollowProjectileStrategy<T> : FlyProjectileStrategy<T> where T : FlyFollowProjectileStrategyData
    {
        protected const float TIME_DELAY = 0.15f;
        protected float currentTime = 0;
        protected IEntityData target;

        public override void Init(ProjectileStrategyData projectileStrategyData, Projectile controllerProjectile, Vector2 direction, Vector2 originalPosition, Vector2 targetPosition, IEntityData targetData = null)
        {
            base.Init(projectileStrategyData, controllerProjectile, direction, originalPosition, targetPosition, targetData);
            target = targetData;
        }

        public override void Start()
        {
            currentTime = 0;
            controllerProjectile.UpdateRotation(currentDirection.ToQuaternion());
        }

        public override void Update()
        {
            base.Update();

            controllerProjectile.UpdatePositionBySpeed(strategyData.moveSpeed, currentDirection);

            if (currentTime > TIME_DELAY)
            {
                currentTime = 0;
                if (target != null && !target.IsDead)
                {
                    var currentAngle = CalculateCurrentDegree();
                    if (strategyData.offsetDegree <= 0 || Mathf.Abs(currentAngle) >= strategyData.offsetDegree)
                    {
                        var offset = currentAngle > 0 ? strategyData.offsetDegree : -strategyData.offsetDegree;
                        currentDirection = Quaternion.AngleAxis(offset, controllerProjectile.Direction) * currentDirection;
                    }
                    else currentDirection = (target.Position - controllerProjectile.CenterPosition).normalized;
                    controllerProjectile.UpdateRotation(currentDirection.ToQuaternion());
                }
            }
            else currentTime += Time.deltaTime;
        }

        private float CalculateCurrentDegree()
            => Vector2.SignedAngle(currentDirection, target.Position - controllerProjectile.CenterPosition);

        protected override void HitTarget(IEntityData target, Vector2 hitPoint, Vector2 hitDirection)
        {
            base.HitTarget(target, hitPoint, hitDirection);

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
    }
}