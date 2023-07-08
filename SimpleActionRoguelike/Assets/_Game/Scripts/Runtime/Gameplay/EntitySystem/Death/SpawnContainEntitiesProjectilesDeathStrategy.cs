using Cysharp.Threading.Tasks;
using Runtime.ConfigModel;
using System.Threading;
using UnityEngine;
using System.Linq;
using Runtime.Definition;
using Runtime.Message;
using Runtime.Core.Message;

namespace Runtime.Gameplay.EntitySystem
{
    public class SpawnContainEntitiesProjectilesDeathStrategy : DeathStrategy<SpawnContainEntitiesProjectilesDeathDataConfigItem>
    {
        protected override UniTask Execute(IEntityData entityData, SpawnContainEntitiesProjectilesDeathDataConfigItem deathDataConfig, CancellationToken cancellationToken)
        {
            var numberOfProjectiles = deathDataConfig.projectileNumber;

            var bigAngle = 360;
            var projectileCenterAngleOffset = (float)bigAngle / numberOfProjectiles;
            var firstDegree = 0;
            var firstDirection = Vector2.up;

            Vector2 projectilePosition = entityData.Position;
            float offset = 0.3f; // avoid collide obstacle immediately when spawn
            for (int i = 0; i < numberOfProjectiles; i++)
            {
                var direction = (Quaternion.AngleAxis(firstDegree + projectileCenterAngleOffset * i, Vector3.forward) * firstDirection).normalized;
                SpawnProjectileAsync(entityData, deathDataConfig, direction, projectilePosition + (Vector2)(offset * direction), cancellationToken).Forget();
            }
            return UniTask.CompletedTask;
        }

        private async UniTaskVoid SpawnProjectileAsync(IEntityData entityData, SpawnContainEntitiesProjectilesDeathDataConfigItem dataConfig, Vector2 direction, Vector2 projectilePosition, CancellationToken cancellationToken)
        {
            EntitiesManager.Instance.CurrentActionContainEnemySpawn++;

            var spawnEntities = dataConfig.spawnEntities;
            if (dataConfig.useOwnerLevel)
                spawnEntities = spawnEntities.Select(x => new SpawnedEntityInfo(x.entityId, x.entityType, entityData.Level, x.entityNumber)).ToArray();

            FlyFowardSpawnEntitiesProjectileStrategyData flyForwardProjectileStrategyData = new FlyFowardSpawnEntitiesProjectileStrategyData(
                                                                                        spawnEntities,
                                                                                        () => EntitiesManager.Instance.CurrentActionContainEnemySpawn--,
                                                                                        cancellationToken,
                                                                                        dataConfig.projectileMoveDistance,
                                                                                        dataConfig.projectileMoveSpeed,
                                                                                        _ => OnProjectileCallback(_, dataConfig, entityData));

            var projectileGameObject = await EntitiesManager.Instance.CreateProjectileAsync(dataConfig.projectileId, entityData, projectilePosition, cancellationToken);
            var projectile = projectileGameObject.GetComponent<Projectile>();

            var projectileStrategy = ProjectileStrategyFactory.GetProjectileStrategy(ProjectileStrategyType.FlyForwardSpawnEntities);
            projectileStrategy.Init(flyForwardProjectileStrategyData, projectile, direction, default, projectilePosition);
            projectile.InitStrategy(projectileStrategy);
        }

        private void OnProjectileCallback(ProjectileCallbackData callbackData, SpawnContainEntitiesProjectilesDeathDataConfigItem config, IEntityData entityData)
        {
            SimpleMessenger.Publish(MessageScope.EntityMessage, new SentDamageMessage(
                EffectSource.None,
                EffectProperty.Normal,
                config.projectileDamageBonus,
                config.projectileDamageFactors,
                entityData,
                callbackData.target
            ));
        }
    }
}