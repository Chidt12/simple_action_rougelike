using System;
using System.Threading;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public interface IProjectileStrategy : IDisposable
    {
        void Init(ProjectileStrategyData projectileStrategyData, Projectile controllerProjectile, Vector2 direction, Vector2 originalPosition, IEntityPositionData creatorData = null);
        void Start();
        void Update();
        void Collide(Collider2D collider);
    }

    public abstract class ProjectileStrategyData 
    {
        public EffectProperty effectProperty;
        public EffectSource damageSource;

        public ProjectileStrategyData(EffectSource damageSource, EffectProperty effectProperty)
        {
            this.damageSource = damageSource;
            this.effectProperty = effectProperty;
        }
    }
}
