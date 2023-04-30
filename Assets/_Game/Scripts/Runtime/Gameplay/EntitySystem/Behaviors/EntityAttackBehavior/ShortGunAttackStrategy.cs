using Cysharp.Threading.Tasks;
using Runtime.Helper;
using System.Threading;
using UnityEngine;
using System.Linq;
using Runtime.Definition;

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
            triggerActionEventProxy.TriggerEvent(AnimationType.Attack1, cancellationToken,
                    stateAction: data => {
                        FireProjectiles(data.spawnVFXPoints, cancellationToken);
                    },
                    endAction: data => {
                        _isShooting = false;
                    });
            await UniTask.WaitUntil(() => !_isShooting, cancellationToken: cancellationToken);
        }

        private void FireProjectiles(Transform[] spawnPoints, CancellationToken cancellationToken)
        {
            var controlData = (IEntityControlData)creatorData;
            Vector2 direction = Vector2.zero;
            if (controlData != null)
                direction = (controlData.FaceDirection).normalized;

            SpawnProjectileAsync(direction, spawnPoints, cancellationToken).Forget();
        }

        private async UniTaskVoid SpawnProjectileAsync(Vector2 direction, Transform[] spawnPoints, CancellationToken cancellationToken)
        {
            var positionData = (IEntityPositionData)creatorData;
            if (positionData != null)
            {
                FlyForwardProjectileStrategyData flyForwardProjectileStrategyData = null;
                flyForwardProjectileStrategyData = new FlyForwardProjectileStrategyData(EffectSource.FromNormalAttack,
                                                                                        EffectProperty.Normal,
                                                                                            ownerWeaponModel.AttackRange,
                                                                                            ownerWeaponModel.ProjectileSpeed,
                                                                                            damageBonus: ownerWeaponModel.DamageBonus,
                                                                                            damageFactors: ownerWeaponModel.DamageFactors);

                var suitablePosition = spawnPoints.Select(x => x.position).ToList().GetSuitableValue(positionData.Position);
                var projectileGameObject = await EntitiesManager.Instance.CreateProjectileAsync(ownerWeaponModel.ProjectileId, positionData, suitablePosition, cancellationToken);
                var projectile = projectileGameObject.GetOrAddComponent<Projectile>();
                var projectileStrategy = ProjectileStrategyFactory.GetProjectilStrategy(ProjectileStrategyType.FlyForward);
                projectileStrategy.Init(flyForwardProjectileStrategyData, projectile, direction, suitablePosition, positionData);
                projectile.InitStrategy(projectileStrategy);
            }
        }

        protected override UniTask TriggerSpecialAttack(CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }
    }
}
