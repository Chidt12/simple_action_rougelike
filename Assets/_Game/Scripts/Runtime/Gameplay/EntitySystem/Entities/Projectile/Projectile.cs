using Cysharp.Threading.Tasks;
using Runtime.Core.Pool;
using System.Threading;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public class Projectile : Disposable, IProjectile
    {
        [SerializeField]
        protected string impactPrefabName;
        protected IProjectileStrategy currentStrategy;
        protected float adjustSpeedFactor;
        protected CancellationTokenSource cancellationTokenSource;

        public string ImpactPrefabName => impactPrefabName;
        public bool GeneratedImpact { get; set; }
        public IEntityPositionData Creator { get; private set; }
        public Vector2 CreatorPosition => Creator.Position;
        public Vector2 CenterPosition => transform.position;
        public Vector2 Direction => transform.up;

        private void Update() => currentStrategy?.Update();

        public virtual UniTask BuildAsync(IEntityPositionData creatorData, Vector3 position)
        {
            GeneratedImpact = false;
            transform.position = position;
            Creator = creatorData;
            currentStrategy = null;
            cancellationTokenSource = new CancellationTokenSource();
            return UniTask.CompletedTask;
        }

        public void UpdatePositionBySpeed(float speed, Vector3 direction)
            => transform.position = transform.position + speed * (1 + adjustSpeedFactor) * direction.normalized * Time.deltaTime;

        public void UpdateAdjustSpeedFactor(float adjustSpeedFactor) => this.adjustSpeedFactor = adjustSpeedFactor;
        public void UpdateRotation(Quaternion rotation)
            => transform.rotation = rotation;

        public override void Dispose()
        {
            currentStrategy?.Dispose();
            cancellationTokenSource?.Cancel();
        }

        public void InitStrategy(IProjectileStrategy projectileStrategy)
        {
            if (projectileStrategy == null)
                CompleteStrategy(true);
            else
            {
                currentStrategy = projectileStrategy;
                currentStrategy.Start();
            }
        }

        public void CompleteStrategy(bool displayImpact)
        {
            DestroySelf(displayImpact);
        }

        protected void DestroySelf(bool displayImpact)
        {
            if (displayImpact)
                GenerateImpact(transform.position).Forget();
            gameObject.transform.parent = null;
            PoolManager.Instance.Return(gameObject);
        }

        protected async UniTaskVoid GenerateImpact(Vector3 position)
        {
            if (string.IsNullOrEmpty(impactPrefabName) || GeneratedImpact)
                return;

            var impact = await PoolManager.Instance.Rent(impactPrefabName, token: cancellationTokenSource.Token);
            impact.transform.position = position;
        }
    }

}