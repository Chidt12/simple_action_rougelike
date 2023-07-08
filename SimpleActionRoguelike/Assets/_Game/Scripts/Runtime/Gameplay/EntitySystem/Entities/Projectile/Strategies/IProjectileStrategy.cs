using System;
using System.Threading;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public interface IProjectileStrategy : IDisposable
    {
        void Init(ProjectileStrategyData projectileStrategyData, Projectile controllerProjectile, Vector2 direction, Vector2 originalPosition, Vector2 targetPosition, IEntityData creatorData = null);
        void Start();
        void Update();
        void Collide(Collider2D collider);
        void Complete(bool forceComplete, bool displayImpact);
    }

    public abstract class ProjectileStrategyData 
    {
        public Action<ProjectileCallbackData> callbackAction;

        public ProjectileStrategyData(Action<ProjectileCallbackData> callbackAction)
        {
            this.callbackAction = callbackAction;
        }
    }

    public class ProjectileCallbackData
    {
        public Vector2 hitPoint;
        public Vector2 direction;
        public IEntityData target;

        public ProjectileCallbackData(Vector2 hitPoint, Vector2 direction, IEntityData target)
        {
            this.hitPoint = hitPoint;
            this.direction = direction;
            this.target = target;
        }
    }
}
