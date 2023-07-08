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

        public IEntityData Creator { get; private set; }
        public Vector2 CreatorPosition => Creator.Position;
        public Vector2 CenterPosition => transform.position;
        public Vector2 Direction => transform.up;

        private void Update() => currentStrategy?.Update();

        public virtual UniTask BuildAsync(IEntityData creatorData, Vector3 position)
        {
            transform.position = position;
            Creator = creatorData;
            currentStrategy = null;
            cancellationTokenSource = new CancellationTokenSource();
            return UniTask.CompletedTask;
        }

        public void UpdatePosition(Vector2 position)
        {
            transform.position = position;
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

        public virtual void InitStrategy(IProjectileStrategy projectileStrategy)
        {
            if (currentStrategy != null)
                currentStrategy.Complete(true, false);

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

        public async UniTaskVoid GenerateImpact(Vector3 position)
        {
            if (string.IsNullOrEmpty(impactPrefabName))
                return;
            var impact = await PoolManager.Instance.Rent(impactPrefabName, token: cancellationTokenSource.Token);
            impact.transform.position = position;
        }
    }

}