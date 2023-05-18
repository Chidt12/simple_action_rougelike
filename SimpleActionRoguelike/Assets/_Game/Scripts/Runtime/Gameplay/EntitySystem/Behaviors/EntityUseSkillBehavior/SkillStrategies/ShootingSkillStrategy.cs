using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using System.Linq;
using Runtime.Helper;
using Runtime.Core.Message;
using Runtime.Message;
using Runtime.Definition;
using Runtime.ConfigModel;

namespace Runtime.Gameplay.EntitySystem
{
    public class ShootingSkillStrategy : SkillStrategy<ShootingSkillModel>
    {
        public override bool CheckCanUseSkill()
        {
            if (ownerModel.DependTarget)
            {
                if(creatorData.Target != null && !creatorData.Target.IsDead)
                {
                    var distance = Vector2.Distance(creatorData.Position, creatorData.Target.Position);
                    return distance <= ownerModel.ProjectileMoveDistance;
                }
                return false;
            }
            return true;
        }

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
            flyForwardProjectileStrategyData = new FlyForwardProjectileStrategyData(ownerModel.ProjectileMoveDistance,
                                                                                    ownerModel.ProjectileMoveSpeed,
                                                                                    ProjectileCallback);

            var projectileGameObject = await EntitiesManager.Instance.CreateProjectileAsync(ownerModel.ProjectileId, creatorData, spawnPosition, cancellationToken);
            var projectile = projectileGameObject.GetOrAddComponent<Projectile>();
            var projectileStrategy = ProjectileStrategyFactory.GetProjectilStrategy(ownerModel.ProjectileStrategyType);
            projectileStrategy.Init(flyForwardProjectileStrategyData, projectile, direction, spawnPosition, creatorData);
            projectile.InitStrategy(projectileStrategy);
        }

        private void ProjectileCallback(ProjectileCallbackData callbackData)
        {
            SimpleMessenger.Publish(MessageScope.EntityMessage, new SentDamageMessage(
                EffectSource.FromSkill,
                EffectProperty.Normal,
                ownerModel.ProjectileDamageBonus,
                ownerModel.ProjectileDamageFactors,
                creatorData,
                callbackData.target
            ));

            var targetStatusData = (IEntityStatusData)callbackData.target;
            if(targetStatusData != null)
            {
                SimpleMessenger.Publish(MessageScope.EntityMessage, new SentStatusEffectMessage(
                    creatorData,
                    targetStatusData,
                    new StatusIdentity(0, StatusType.Stun)
                ));
            }
        }
    }
}