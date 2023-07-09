using Cysharp.Threading.Tasks;
using Runtime.ConfigModel;
using Runtime.Core.Message;
using Runtime.Core.Pool;
using Runtime.Definition;
using Runtime.Message;

namespace Runtime.Gameplay.EntitySystem
{
    public class BoomerangArtifactSystem : RuneArtifactSystem<BoomerangArtifactDataConfigItem>
    {
        public override ArtifactType ArtifactType => ArtifactType.Boomerang;

        public override bool Trigger()
        {
            base.Trigger();
            FireProjectileAsync().Forget();

            return true;
        }

        private async UniTaskVoid FireProjectileAsync()
        {
            var projectileGameObject = await EntitiesManager.Instance.CreateProjectileAsync(ownerData.projectileId, ownerEntityData, ownerEntityData.Position, cancellationTokenSource.Token);
            var projectile = projectileGameObject.GetComponent<Projectile>();
            var projectileStrategyData = new FlyZigzagProjectileStrategyData(ownerData.numberOfHits, ownerData.flyDistance, ownerData.flySpeed, OnCallback);
            var projectileStrategy = ProjectileStrategyFactory.GetProjectileStrategy(ProjectileStrategyType.FlyZigzag);
            projectileStrategy.Init(projectileStrategyData, projectile, ((IEntityControlData)ownerEntityData).FaceDirection, ownerEntityData.Position, default, ownerEntityData);
            projectile.InitStrategy(projectileStrategy);
        }

        private void OnCallback(ProjectileCallbackData callbackData)
        {
            SimpleMessenger.Publish(MessageScope.EntityMessage, new SentDamageMessage(
                EffectSource.FromArtifact,
                EffectProperty.Normal,
                ownerData.damageBonus,
                ownerData.damageFactors,
                ownerEntityData,
                callbackData.target
            ));

            var targetStatusData = (IEntityStatusData)callbackData.target;
            if (targetStatusData != null)
            {
                SimpleMessenger.Publish(MessageScope.EntityMessage, new SentStatusEffectMessage(
                    ownerEntityData,
                    targetStatusData,
                    ownerData.triggeredStatus
                ));
            }
        }
    }
}