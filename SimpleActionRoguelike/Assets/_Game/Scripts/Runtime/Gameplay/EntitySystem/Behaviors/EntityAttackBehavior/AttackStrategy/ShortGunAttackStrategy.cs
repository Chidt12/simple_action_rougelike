using Cysharp.Threading.Tasks;
using Runtime.Helper;
using System.Threading;
using UnityEngine;
using System.Linq;
using Runtime.Definition;
using Runtime.Core.Message;
using Runtime.Message;
using System;

namespace Runtime.Gameplay.EntitySystem
{
    public class ShortGunAttackStrategy : AttackStrategy<ShortGunWeaponModel>
    {
        private bool _isShooting;

        public override bool CheckCanAttack()
            => base.CheckCanAttack() && !_isShooting;

        protected override async UniTask TriggerAttack(CancellationToken cancellationToken)
        {
            _isShooting = true;
            triggerActionEventProxy.TriggerEvent(AnimationType.Attack1,
                    stateAction: data => {
                        FireProjectiles(ownerWeaponModel.NumberOfProjectilesInHorizontal, 30, data.spawnVFXPoints, cancellationToken);
                    },
                    endAction: data => {
                        _isShooting = false;
                    });
            await UniTask.WaitUntil(() => !_isShooting, cancellationToken: cancellationToken);
        }

        private void FireProjectiles(int numberOfProjectiles, float angleBetweenTwoProjectiles, Transform[] spawnPoints, CancellationToken cancellationToken)
        {
            var suitableFirePosition = GetSuitableSpawnPosition(spawnPoints);

            var bigAngle = (numberOfProjectiles - 1) * angleBetweenTwoProjectiles;
            var firstDegree = -bigAngle / 2;

            var faceDirection = GetFaceDirection();

            for (int i = 0; i < numberOfProjectiles; i++)
            {
                var projectileDirection = (Quaternion.AngleAxis(firstDegree + angleBetweenTwoProjectiles * i, Vector3.forward) * faceDirection).normalized;
                SpawnProjectileAsync(projectileDirection, suitableFirePosition, cancellationToken).Forget();
            }
        }

        private async UniTaskVoid SpawnProjectileAsync(Vector2 direction, Vector2 spawnPoint, CancellationToken cancellationToken)
        {
            for (int i = 0; i < ownerWeaponModel.NumberOfProjectilesInVertical; i++)
            {
                IProjectileStrategy projectileStrategy = null;
                ProjectileStrategyData projectileStrategyData = null;

                if (ownerWeaponModel.GoThrough)
                {
                    projectileStrategyData = new FlyForwardThroughProjecitleStrategyData( true,
                                                                                            ownerWeaponModel.AttackRange,
                                                                                            ownerWeaponModel.ProjectileSpeed,
                                                                                            ProjectileCallback);
                    projectileStrategy = ProjectileStrategyFactory.GetProjectilStrategy(ProjectileStrategyType.FlyForwardThrough);
                }
                else
                {
                    projectileStrategyData = new FlyForwardProjectileStrategyData(ownerWeaponModel.AttackRange,
                                                                                            ownerWeaponModel.ProjectileSpeed,
                                                                                            ProjectileCallback);
                    projectileStrategy = ProjectileStrategyFactory.GetProjectilStrategy(ProjectileStrategyType.FlyForward);
                }



                var projectileGameObject = await EntitiesManager.Instance.CreateProjectileAsync(ownerWeaponModel.ProjectileId, creatorData, spawnPoint, cancellationToken);
                var projectile = projectileGameObject.GetOrAddComponent<Projectile>();
                projectileStrategy.Init(projectileStrategyData, projectile, direction, spawnPoint, creatorData);
                projectile.InitStrategy(projectileStrategy);

                await UniTask.Delay(TimeSpan.FromSeconds(0.15f), cancellationToken: cancellationToken);
            }
        }

        private void ProjectileCallback(ProjectileCallbackData callbackData)
        {
            SimpleMessenger.Publish(MessageScope.EntityMessage, new SentDamageMessage(
                EffectSource.FromNormalAttack,
                EffectProperty.Normal,
                ownerWeaponModel.DamageBonus,
                ownerWeaponModel.DamageFactors,
                creatorData,
                callbackData.target
            ));
        }

        protected override UniTask TriggerSpecialAttack(CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }
    }
}
