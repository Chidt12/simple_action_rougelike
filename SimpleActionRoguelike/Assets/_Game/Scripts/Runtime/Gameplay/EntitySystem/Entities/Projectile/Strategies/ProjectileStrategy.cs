using Cysharp.Threading.Tasks;
using Runtime.Core.Pool;
using System.Threading;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public class ProjectileStrategy<T> : IProjectileStrategy where T : ProjectileStrategyData
    {
        protected T strategyData;
        protected Projectile controllerProjectile;
        protected CancellationTokenSource cancellationTokenSource;

        public virtual void Init(ProjectileStrategyData projectileStrategyData, Projectile controllerProjectile, Vector2 direction, Vector2 originalPosition, IEntityData targetData = null)
        {
            cancellationTokenSource = new CancellationTokenSource();
            strategyData = projectileStrategyData as T;
            this.controllerProjectile = controllerProjectile;
        }

        public virtual void Collide(Collider2D collider) { }

        public virtual void Start() { }

        public virtual void Update() { }

        public void Dispose()
        {
            cancellationTokenSource?.Cancel();
        }

        public void Complete(bool forceComplete, bool displayImpact)
        {
            if (!forceComplete)
                controllerProjectile.CompleteStrategy(displayImpact);
        }
    }
}
