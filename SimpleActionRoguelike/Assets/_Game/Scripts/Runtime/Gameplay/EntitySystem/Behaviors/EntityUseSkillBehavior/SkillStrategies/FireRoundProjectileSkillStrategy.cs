using Cysharp.Threading.Tasks;
using Runtime.Core.Pool;
using Runtime.Helper;
using System;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public class FireRoundProjectileSkillStrategy : SkillStrategy<FireRoundProjectileSkillModel>
    {
        public override bool CheckCanUseSkill()
        {
            if (ownerModel.DependTarget)
            {
                if (creatorData.Target != null && !creatorData.Target.IsDead)
                {
                    var distance = Vector2.Distance(creatorData.Position, creatorData.Target.Position);
                    return distance <= ownerModel.MaxProjectileFlyDistance;
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
                if (ownerModel.DelayBetweenProjectile > 0)
                    await UniTask.Delay(TimeSpan.FromSeconds(ownerModel.DelayBetweenProjectile), cancellationToken: cancellationToken);
            }
        }

        private void FireProjectile(Transform spawnTransform, CancellationToken cancellationToken)
        {
            var fireDirection = ownerModel.FocusTargetDuringExecute ? creatorData.Target.Position - creatorData.Position : creatorData.FaceDirection;
            creatorData.SetFaceDirection(fireDirection);

            var bigAngle = (ownerModel.NumberOfBulletsInProjectile - 1) * ownerModel.AngleBetweenBullet;
            var firstDegree = -bigAngle / 2;

            var distance = ownerModel.DependTarget ? Vector2.Distance(creatorData.Target.Position, spawnTransform.position) : ownerModel.MaxProjectileFlyDistance;

            for (int i = 0; i < ownerModel.NumberOfBulletsInProjectile; i++)
            {
                var bulletFireDirection = (Quaternion.AngleAxis(firstDegree + ownerModel.AngleBetweenBullet * i, Vector3.forward) * fireDirection).normalized;
                var targetPosition = spawnTransform.position + bulletFireDirection * distance;
                SpawnBulletAsync(spawnTransform.position, targetPosition, cancellationToken).Forget();
            }
        }

        private async UniTaskVoid SpawnBulletAsync(Vector2 firePosition, Vector2 targetPosition, CancellationToken cancellationToken)
        {
            FlyRoundProjectileStrategyData projectileStrategyData = null;
            projectileStrategyData = new FlyRoundProjectileStrategyData(ownerModel.ProjectileFlyDuration, ownerModel.ProjectileFlyHeight, ownerModel.WarningPrefabName, ownerModel.DamageAreaHeight, ownerModel.DamageAreaWidth, OnProjectileCallback);

            var projectileGameObject = await EntitiesManager.Instance.CreateProjectileAsync(ownerModel.ProjectileId, creatorData, firePosition, cancellationToken);
            var projectile = projectileGameObject.GetOrAddComponent<Projectile>();
            var projectileStrategy = ProjectileStrategyFactory.GetProjectileStrategy(Definition.ProjectileStrategyType.FlyRound);
            projectileStrategy.Init(projectileStrategyData, projectile, targetPosition - firePosition, firePosition, targetPosition, creatorData);
            projectile.InitStrategy(projectileStrategy);
        }

        private void OnProjectileCallback(ProjectileCallbackData callbackData)
        {
            SpawnDamageBox(callbackData.hitPoint).Forget();
        }

        private async UniTaskVoid SpawnDamageBox(Vector2 spawnPoint)
        {
            var impactObject = await PoolManager.Instance.Rent(ownerModel.ImpactPrefabName);
            var impact = impactObject.GetComponent<AnimatorDamageBox>();
            impactObject.transform.position = spawnPoint;
            impact.Scale(new Vector2(ownerModel.DamageAreaWidth / 2, ownerModel.DamageAreaHeight / 2));
            impact.Init(creatorData, EffectSource.FromSkill, EffectProperty.Normal, 0, ownerModel.FirstInitDamageFactors, default, onTurnOff: () => {
                if(ownerModel.DamageAreaInterval > 0 && ownerModel.DamageAreaLifeTime > 0)
                    SpawnDamageArea(spawnPoint).Forget();
            });
        }

        private async UniTaskVoid SpawnDamageArea(Vector2 spawnPoint)
        {
            var damageAreaData = new DamageAreaData(creatorData, ownerModel.DamageAreaLifeTime, ownerModel.DamageAreaInterval, EffectSource.FromSkill, EffectProperty.Normal, ownerModel.DamageAreaWidth, ownerModel.DamageAreaHeight, ownerModel.DamageAreaDamageFactors);
            await EntitiesManager.Instance.CreateDamageAreaAsync(ownerModel.DamageAreaPrefabName, damageAreaData, creatorData, spawnPoint, default);
        }
    }
}