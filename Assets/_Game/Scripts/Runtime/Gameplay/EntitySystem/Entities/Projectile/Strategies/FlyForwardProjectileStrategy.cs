using Runtime.Core.Message;
using Runtime.Extensions;
using Runtime.Message;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public class FlyForwardProjectileStrategyData : FlyProjectileStrategyData
    {

        public FlyForwardProjectileStrategyData(EffectSource damageSource, EffectProperty damageProperty, float moveDistance, float moveSpeed, float damageBonus = 0,
                                                DamageFactor[] damageFactors = null)
            : base(damageSource, damageProperty, moveDistance, moveSpeed, damageBonus, damageFactors) { }
    }

    public class FlyForwardProjectileStrategy : FlyForwardProjectileStrategy<FlyForwardProjectileStrategyData> { }

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
            SimpleMessenger.Publish(MessageScope.EntityMessage, new SentDamageMessage(
                strategyData.damageSource, 
                strategyData.effectProperty,
                strategyData.damageBonus,
                strategyData.damageFactors,
                controllerProjectile.Creator,
                target)
            );
            CreateImpactEffect(hitPoint);
            controllerProjectile.CompleteStrategy(true);
        }

        #endregion Class Methods
    }
}