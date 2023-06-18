using Cysharp.Threading.Tasks;
using Runtime.ConfigModel;
using Runtime.Core.Message;
using Runtime.Definition;
using Runtime.Helper;
using Runtime.Message;
using System.Threading;
using UnityEngine;
using System.Linq;
using System;

namespace Runtime.Gameplay.EntitySystem
{
    public class FireProjectileAroundSkillStrategy : SkillStrategy<FireProjectileAroundSkillModel>
    {
        public override bool CheckCanUseSkill()
        {
            if (ownerModel.DependTarget)
            {
                if (creatorData.Target != null && !creatorData.Target.IsDead)
                {
                    var distance = Vector2.Distance(creatorData.Position, creatorData.Target.Position);
                    return distance < ownerModel.ProjectileMoveDistance;
                }
                return false;
            }
            return true;
        }

        protected override async UniTask PresentSkillAsync(CancellationToken cancellationToken, int index)
        {
            var direction = creatorData.FaceDirection;

            for (int i = 0; i < ownerModel.WaveNumber; i++)
            {
                var hasFinishedAnimation = false;
                var waveIndex = i;
                entityTriggerActionEventProxy.TriggerEvent(
                    index.GetUseSkillByIndex(),
                    stateAction: callbackData =>
                    {
                        var suitablePosition = callbackData.spawnVFXPoints == null ? (Vector3)creatorData.Position : callbackData.spawnVFXPoints.Select(x => x.position).ToList().GetSuitableValue(creatorData.Position);
                        var numberOfProjectiles = ownerModel.NumberOfProjectiles;
                        var bigAngle = 360;
                        var projectileCenterAngleOffset = (float)bigAngle / numberOfProjectiles;
                        var firstDegree = 0;
                        var firstDirection = MathHelper.RotateVector2(Vector2.up, waveIndex * ownerModel.RotateBetweenWaves);
                        Vector2 projectilePosition = suitablePosition;
                        float offset = 0.3f; // avoid collide obstacle immediately when spawn
                        for (int i = 0; i < numberOfProjectiles; i++)
                        {
                            var direction = (Quaternion.AngleAxis(firstDegree + projectileCenterAngleOffset * i, Vector3.forward) * firstDirection).normalized;
                            FireProjectile(projectilePosition + (Vector2)direction * offset, direction, cancellationToken).Forget();
                        }
                    },
                    endAction: callbackData => hasFinishedAnimation = true
                );

                await UniTask.WaitUntil(() => hasFinishedAnimation, cancellationToken: cancellationToken);
                if(ownerModel.DelayBetweenWaves > 0)
                {
                    await UniTask.Delay(TimeSpan.FromSeconds(ownerModel.DelayBetweenWaves), cancellationToken: cancellationToken);
                }
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
            var projectileStrategy = ProjectileStrategyFactory.GetProjectileStrategy(ownerModel.ProjectileStrategyType);
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
            if (targetStatusData != null)
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