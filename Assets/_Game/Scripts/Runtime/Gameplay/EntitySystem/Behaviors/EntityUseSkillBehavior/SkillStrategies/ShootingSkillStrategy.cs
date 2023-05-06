using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using System.Linq;
using Runtime.Helper;

namespace Runtime.Gameplay.EntitySystem
{
    public class ShootingSkillStrategy : SkillStrategy<ShootingSkillModel>
    {
        protected async override UniTask PresentSkillAsync(CancellationToken cancellationToken, int index)
        {
            var direction = creatorData.FaceDirection;

            for (int i = 0; i < ownerModel.NumberOfProjectiles; i++)
            {
                var hasFinishedAnimation = false;
                entityTriggerActionEventProxy.TriggerEvent(
                    index.GetUseSkillByIndex(),
                    stateAction: callbackData =>
                    {
                        var suitablePosition = callbackData.spawnVFXPoints == null ? (Vector3)creatorData.Position : callbackData.spawnVFXPoints.Select(x => x.position).ToList().GetSuitableValue(creatorData.Position);
                        FireProjectile(suitablePosition, direction, cancellationToken).Forget();
                    },
                    endAction: callbackData => hasFinishedAnimation = true         
                );

                await UniTask.WaitUntil(() => hasFinishedAnimation, cancellationToken: cancellationToken);
            }
        }

        private async UniTaskVoid FireProjectile(Vector2 spawnPosition, Vector2 direction, CancellationToken cancellationToken)
        {
            FlyForwardProjectileStrategyData flyForwardProjectileStrategyData = null;
            flyForwardProjectileStrategyData = new FlyForwardProjectileStrategyData(EffectSource.FromSkill,
                                                                                    EffectProperty.Normal,
                                                                                        ownerModel.ProjectileMoveDistance,
                                                                                        ownerModel.ProjectileMoveSpeed,
                                                                                        damageBonus: ownerModel.ProjectileDamageBonus,
                                                                                        damageFactors: ownerModel.ProjectileDamageFactors);

            var projectileGameObject = await EntitiesManager.Instance.CreateProjectileAsync(ownerModel.ProjectileId, creatorData, spawnPosition, cancellationToken);
            var projectile = projectileGameObject.GetOrAddComponent<Projectile>();
            var projectileStrategy = ProjectileStrategyFactory.GetProjectilStrategy(ownerModel.ProjectileStrategyType);
            projectileStrategy.Init(flyForwardProjectileStrategyData, projectile, direction, spawnPosition, creatorData);
            projectile.InitStrategy(projectileStrategy);
        }
    }
}