using System;
using System.Threading;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public interface IProjectileStrategy : IDisposable
    {
        void Init(ProjectileStrategyData projectileStrategyData, Projectile controllerProjectile, Vector2 direction, Vector2 originalPosition, IEntityData creatorData = null);
        void Start();
        void Update();
        void Collide(Collider2D collider);
    }

    public abstract class ProjectileStrategyData 
    {
        public DamageSource damageSource;

        public ProjectileStrategyData(DamageSource damageSource)
        {
            this.damageSource = damageSource;
        }
    }
}
