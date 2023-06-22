using Runtime.ConfigModel;
using Runtime.Definition;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using Runtime.Core.Pool;
using Runtime.Helper;
using Runtime.Core.Message;
using Runtime.Message;

namespace Runtime.Gameplay.EntitySystem
{
    public class AutoStableGunArtifactSystem : RuneArtifactSystem<AutoStableGunArtifactDataConfigItem>
    {
        private List<AutoStableGun> _listGuns;

        public override ArtifactType ArtifactType => ArtifactType.AutoStableGun;


        public async override UniTask Init(IEntityData entityData)
        {
            await base.Init(entityData);
            _listGuns = new();
        }

        protected override void OnUpdateAsync()
        {
            base.OnUpdateAsync();
            foreach (var gun in _listGuns)
            {
                gun.OnUpdate(Time.deltaTime);
                if (gun.CanShooting())
                {
                    gun.Shooting();
                }
            }
        }

        public override void Trigger()
        {
            base.Trigger();
            SpawnGunAsync().Forget();
        }

        private async UniTaskVoid SpawnGunAsync()
        {
            var gunObject = await PoolManager.Instance.Rent(ownerData.gunPrefabName);
            var autoGun = gunObject.GetComponent<AutoStableGun>();
            autoGun.Init(ownerData.range, ownerData.interval, new[] { EntityType.Enemy, EntityType.Boss }, OnShooting);
        }

        private void OnShooting(Transform spawnPoint, Vector2 direction)
        {
            FireProjectileAsync(spawnPoint.position, direction).Forget();
        }

        private async UniTaskVoid FireProjectileAsync(Vector2 spawnPosition, Vector2 direction)
        {
            FlyForwardProjectileStrategyData flyForwardProjectileStrategyData = null;
            flyForwardProjectileStrategyData = new FlyForwardProjectileStrategyData(ownerData.range,
                                                                                    ownerData.projectileSpeed,
                                                                                    ProjectileCallback);

            var projectileGameObject = await EntitiesManager.Instance.CreateProjectileAsync(ownerData.projectileId, ownerEntityData, spawnPosition, cancellationTokenSource.Token);
            var projectile = projectileGameObject.GetOrAddComponent<Projectile>();
            var projectileStrategy = ProjectileStrategyFactory.GetProjectileStrategy(ProjectileStrategyType.FlyForward);
            projectileStrategy.Init(flyForwardProjectileStrategyData, projectile, direction, spawnPosition, ownerEntityData);
            projectile.InitStrategy(projectileStrategy);
        }

        private void ProjectileCallback(ProjectileCallbackData callbackData)
        {
            SimpleMessenger.Publish(MessageScope.EntityMessage, new SentDamageMessage(
                EffectSource.None,
                EffectProperty.Normal,
                ownerData.damageBonus,
                ownerData.damageFactors,
                ownerEntityData,
                callbackData.target
            ));
        }
    }
}