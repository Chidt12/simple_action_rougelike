using Cysharp.Threading.Tasks;
using Runtime.ConfigModel;
using Runtime.Core.Message;
using Runtime.Definition;
using Runtime.Message;
using System.Threading;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public class SpawnForwardProjectilesDeathStrategy : DeathStrategy<SpawnForwardProjectilesDeathDataConfigItem>
    {
        protected override UniTask Execute(IEntityData entityData, SpawnForwardProjectilesDeathDataConfigItem deathDataConfig, CancellationToken cancellationToken)
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
                SpawnProjectileAsync(entityData, deathDataConfig, direction, projectilePosition + (Vector2)direction * offset, cancellationToken).Forget();
            }

            return UniTask.CompletedTask;
        }

        private async UniTaskVoid SpawnProjectileAsync(IEntityData entityData, SpawnForwardProjectilesDeathDataConfigItem dataConfig, Vector2 direction, Vector2 projectilePosition, CancellationToken cancellationToken)
        {
            FlyForwardProjectileStrategyData flyForwardProjectileStrategyData = null;
            flyForwardProjectileStrategyData = new FlyForwardProjectileStrategyData(
                                                        dataConfig.projectileMoveDistance,
                                                        dataConfig.projectileMoveSpeed,
                                                        _ => OnProjectileCallback(_, dataConfig, entityData));

            var projectileGameObject = await EntitiesManager.Instance.CreateProjectileAsync(dataConfig.projectileId, entityData, projectilePosition, cancellationToken);
            var projectile = projectileGameObject.GetComponent<Projectile>();
            var projectileStrategy = ProjectileStrategyFactory.GetProjectileStrategy(ProjectileStrategyType.FlyForward);
            projectileStrategy.Init(flyForwardProjectileStrategyData, projectile, direction, default, projectilePosition);
            projectile.InitStrategy(projectileStrategy);
        }

        private void OnProjectileCallback(ProjectileCallbackData callbackData, SpawnForwardProjectilesDeathDataConfigItem config, IEntityData entityData)
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