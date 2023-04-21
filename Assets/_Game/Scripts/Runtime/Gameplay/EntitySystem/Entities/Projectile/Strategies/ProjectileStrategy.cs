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

        protected virtual void CreateImpactEffect(Vector2 impactEffectPosition)
        {
            var impactEffectName = string.IsNullOrEmpty(controllerProjectile.ImpactPrefabName) ? string.Empty : controllerProjectile.ImpactPrefabName;
            SpawnImpactEffectAsync(impactEffectName, impactEffectPosition).Forget();
        }

        protected virtual async UniTask SpawnImpactEffectAsync(string impactEffectName, Vector2 impactEffectPosition)
        {
            var impactEffect = await PoolManager.Instance.Rent(impactEffectName, token: cancellationTokenSource.Token);
            impactEffect.transform.position = impactEffectPosition;
        }

        public void Dispose()
        {
            cancellationTokenSource?.Cancel();
        }
    }
}
