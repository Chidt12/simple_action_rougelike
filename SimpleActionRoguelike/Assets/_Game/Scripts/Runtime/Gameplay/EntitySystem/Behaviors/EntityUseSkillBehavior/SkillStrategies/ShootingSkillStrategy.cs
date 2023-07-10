using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using System.Linq;
using Runtime.Helper;
using Runtime.Core.Message;
using Runtime.Message;
using System;
using UnityRandom = UnityEngine.Random;
using Runtime.Constants;

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
                    if (distance <= ownerModel.ProjectileMoveDistance)
                    {
                        var direction = creatorData.Target.Position - creatorData.Position;
                        var rayCastChecks = Physics2D.RaycastAll(creatorData.Position, direction, distance);

                        foreach (var rayCastCheck in rayCastChecks)
                        {
                            if (rayCastCheck.collider.gameObject.layer == Layers.OBJECT_LAYER)
                                return false;
                        }

                        return true;
                    }
                }
                return false;
            }
            return true;
        }

        protected async override UniTask PresentSkillAsync(CancellationToken cancellationToken, int index)
        {
            for (int i = 0; i < ownerModel.NumberOfProjectiles; i++)
            {
                var hasFinishedAnimation = false;
                entityTriggerActionEventProxy.TriggerEvent(
                    index.GetUseSkillByIndex(),
                    stateAction: callbackData =>
                    {
                        var fireTransform = callbackData.spawnVFXPoints == null ? creatorData.EntityTransform : callbackData.spawnVFXPoints.ToList().GetSuitableValue(creatorData.EntityTransform);
                        FireProjectile(fireTransform, cancellationToken);
                    },
                    endAction: callbackData => hasFinishedAnimation = true         
                );

                await UniTask.WaitUntil(() => hasFinishedAnimation, cancellationToken: cancellationToken);
                if (ownerModel.DelayBetweenProjectiles > 0)
                    await UniTask.Delay(TimeSpan.FromSeconds(ownerModel.DelayBetweenProjectiles), cancellationToken: cancellationToken);
            }
        }

        private void FireProjectile(Transform fireTransform, CancellationToken cancellationToken)
        {
            var randomFireDeflectionAngle = UnityRandom.Range(-ownerModel.FireDeflectionAngle, ownerModel.FireDeflectionAngle);
            var originDirection = ownerModel.FocusTargetDuringExecute ? creatorData.Target.Position - creatorData.Position : creatorData.FaceDirection;
            var fireDirection = (Quaternion.AngleAxis(randomFireDeflectionAngle, Vector3.forward) * originDirection).normalized;
            creatorData.SetFaceDirection(fireDirection);

            var bigAngle = (ownerModel.NumberOfBulletsInProjectile - 1) * ownerModel.AngleBetweenBullets;
            var firstDegree = -bigAngle / 2;

            for (int i = 0; i < ownerModel.NumberOfBulletsInProjectile; i++)
            {
                var bulletFireDirection = (Quaternion.AngleAxis(firstDegree + ownerModel.AngleBetweenBullets * i, Vector3.forward) * fireDirection).normalized;
                SpawnBulletAsync(fireTransform, bulletFireDirection, cancellationToken).Forget();
            }

        }

        private async UniTaskVoid SpawnBulletAsync(Transform fireTransform, Vector2 fireDirection, CancellationToken cancellationToken)
        {
            FlyForwardProjectileStrategyData flyForwardProjectileStrategyData = null;
            flyForwardProjectileStrategyData = new FlyForwardProjectileStrategyData(ownerModel.ProjectileMoveDistance,
                                                                                    ownerModel.ProjectileMoveSpeed,
                                                                                    ProjectileCallback);

            var projectileGameObject = await EntitiesManager.Instance.CreateProjectileAsync(ownerModel.ProjectileId, creatorData, fireTransform.position, cancellationToken);
            var projectile = projectileGameObject.GetOrAddComponent<Projectile>();
            var projectileStrategy = ProjectileStrategyFactory.GetProjectileStrategy(ownerModel.ProjectileStrategyType);
            projectileStrategy.Init(flyForwardProjectileStrategyData, projectile, fireDirection, fireTransform.position, default, creatorData);
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

            //var targetStatusData = (IEntityStatusData)callbackData.target;
            //if(targetStatusData != null)
            //{
            //    SimpleMessenger.Publish(MessageScope.EntityMessage, new SentStatusEffectMessage(
            //        creatorData,
            //        targetStatusData,
            //        new StatusIdentity(0, StatusType.Stun)
            //    ));
            //}
        }
    }
}