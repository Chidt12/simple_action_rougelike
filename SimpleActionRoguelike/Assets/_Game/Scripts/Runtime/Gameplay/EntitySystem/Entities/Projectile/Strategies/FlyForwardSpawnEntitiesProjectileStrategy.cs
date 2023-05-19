using Cysharp.Threading.Tasks;
using Runtime.ConfigModel;
using System;
using System.Threading;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public class FlyFowardSpawnEntitiesProjectileStrategyData : FlyForwardProjectileStrategyData
    {
        public readonly CancellationToken CancellationToken;
        public readonly SpawnedEntityInfo[] SpawnedEntitiesInfo;
        public readonly Action SpawnedEntitiesAction;

        public FlyFowardSpawnEntitiesProjectileStrategyData(SpawnedEntityInfo[] spawnedEntitiesInfo, Action spawnedEntitiesAction, CancellationToken cancellationToken, float moveDistance, float moveSpeed, Action<ProjectileCallbackData> callbackAction)
            : base(moveDistance, moveSpeed, callbackAction)
        {
            CancellationToken = cancellationToken;
            SpawnedEntitiesInfo = spawnedEntitiesInfo;
            SpawnedEntitiesAction = spawnedEntitiesAction;
        }
    }

    public class FlyForwardSpawnEntitiesProjectileStrategy : FlyForwardProjectileStrategy<FlyFowardSpawnEntitiesProjectileStrategyData>
    {
        private static readonly float s_entitySpawnFromCharacterOffset = 0.1f;

        protected override void HitTarget(IEntityData target, Vector2 hitPoint, Vector2 hitDirection)
        {
            base.HitTarget(target, hitPoint, hitDirection);
        }

        protected override void CollidedDeathTarget()
        {
            SpawnEntitiesAsync().Forget();
            base.CollidedObstacle();
        }

        protected override void CollidedObstacle()
        {
            SpawnEntitiesAsync().Forget();
            base.CollidedObstacle();
        }

        protected override void ReachedTheLifeDistance()
        {
            SpawnEntitiesAsync().Forget();
            base.ReachedTheLifeDistance();
        }

        private async UniTaskVoid SpawnEntitiesAsync()
        {
            await EntitiesManager.Instance.CreateEntitiesAsync(controllerProjectile.CenterPosition, s_entitySpawnFromCharacterOffset, false,
                                                         strategyData.CancellationToken, strategyData.SpawnedEntitiesInfo);
            strategyData.SpawnedEntitiesAction?.Invoke();
        }
    }
}