using Runtime.Helper;
using System;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public class FlyBoomerangProjectileStrategyData : FlyProjectileStrategyData
    {
        public bool goThrough;
        public Action cameBackAction;

        public FlyBoomerangProjectileStrategyData(Action cameBackAction, bool goThrough, float moveDistance, float moveSpeed, Action<ProjectileCallbackData> callbackAction) 
            : base(moveDistance, moveSpeed, callbackAction)
        {
            this.cameBackAction = cameBackAction;
            this.goThrough = goThrough;
        }
    }

    public class FlyBoomerangProjectileStrategy : FlyProjectileStrategy<FlyBoomerangProjectileStrategyData>
    {
        private float _offset = 0.5f;
        private bool _isGoingBack;

        public override void Init(ProjectileStrategyData projectileStrategyData, Projectile controllerProjectile, Vector2 direction, Vector2 originalPosition, Vector2 targetPosition, IEntityData targetData = null)
        {
            base.Init(projectileStrategyData, controllerProjectile, direction, originalPosition, targetPosition, targetData);
            _isGoingBack = false;
        }

        public override void Start()
        {
            base.Start();
            controllerProjectile.UpdateRotation(currentDirection.ToQuaternion());
        }

        public override void Update()
        {
            if (!_isGoingBack)
            {
                controllerProjectile.UpdatePositionBySpeed(strategyData.moveSpeed, currentDirection);
                base.Update();
            }
            else
            {
                currentDirection = controllerProjectile.CreatorPosition - controllerProjectile.CenterPosition;
                controllerProjectile.UpdatePositionBySpeed(strategyData.moveSpeed, currentDirection);
                if(currentDirection.magnitude <= _offset)
                {
                    strategyData.cameBackAction?.Invoke();
                    Complete(false, false);
                }
            }
        }

        protected override void ReachedTheLifeDistance()
        {
            _isGoingBack = true;
        }

        protected override void CollidedObstacle()
        {
            if (!strategyData.goThrough)
                _isGoingBack = true;
        }

        protected override void HitTarget(IEntityData target, Vector2 hitPoint, Vector2 hitDirection)
        {
            controllerProjectile.GenerateImpact(hitPoint).Forget();
            strategyData.callbackAction?.Invoke(new ProjectileCallbackData(hitDirection, hitDirection, target));
            if (!strategyData.goThrough)
                _isGoingBack = true;
        }

        protected override void CollidedDeathTarget()
        {}
    }
}