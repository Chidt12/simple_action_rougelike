using Cysharp.Threading.Tasks;
using Runtime.Core.Message;
using Runtime.Definition;
using Runtime.Helper;
using Runtime.Message;
using System.Threading;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public class BoomerangAttackStrategy : AttackStrategy<BoomerangWeaponModel>
    {
        [SerializeField]
        private GameObject _disableWhenNoProjectiles;

        private int _currentAvailable;
        private bool _isShooting;

        public override bool CheckCanAttack()
        {
            return base.CheckCanAttack() && _currentAvailable > 0;
        }

        public override void Init(WeaponModel weaponModel, IEntityStatData entityData, Transform creatorTransform)
        {
            base.Init(weaponModel, entityData, creatorTransform);

            _currentAvailable = ownerWeaponModel.NumberOfProjectiles;
        }

        protected async override UniTask TriggerAttack(CancellationToken cancellationToken)
        {
            _isShooting = true;
            _currentAvailable--;
            triggerActionEventProxy.TriggerEvent(AnimationType.Attack1,
                    stateAction: data => {
                        UpdateVisual();
                        FireProjectile(data.spawnVFXPoints, cancellationToken).Forget();
                    },
                    endAction: data => {
                        _isShooting = false;
                    });
            await UniTask.WaitUntil(() => !_isShooting, cancellationToken: cancellationToken);
        }

        private void UpdateVisual()
        {
            _disableWhenNoProjectiles.SetActive(_currentAvailable > 0);
        }

        private async UniTaskVoid FireProjectile(Transform[] spawnVFXPoints, CancellationToken token)
        {
            IProjectileStrategy projectileStrategy = null;
            ProjectileStrategyData projectileStrategyData = null;

            projectileStrategy = ProjectileStrategyFactory.GetProjectileStrategy(ProjectileStrategyType.FlyBoomerang);
            projectileStrategyData = new FlyBoomerangProjectileStrategyData(ProjectileCameback, ownerWeaponModel.GoThrough, creatorData.GetTotalStatValue(StatType.AttackRange), ownerWeaponModel.ProjectileSpeed, ProjectileCallback);

            var spawnPoint = GetSuitableSpawnPosition(spawnVFXPoints);
            var projectileGameObject = await EntitiesManager.Instance.CreateProjectileAsync(ownerWeaponModel.ProjectileId, creatorData, spawnPoint, token);
            var projectile = projectileGameObject.GetOrAddComponent<Projectile>();

            var faceDirection = GetFaceDirection();

            projectileStrategy.Init(projectileStrategyData, projectile, faceDirection, spawnPoint, creatorData);
            projectile.InitStrategy(projectileStrategy);
        }

        private void ProjectileCameback()
        {
            _currentAvailable++;
            UpdateVisual();
        }

        private void ProjectileCallback(ProjectileCallbackData callbackData)
        {
            SimpleMessenger.Publish(MessageScope.EntityMessage, new SentDamageMessage(
                EffectSource.FromNormalAttack,
                EffectProperty.Normal,
                0,
                new[] {new DamageFactor(StatType.AttackDamage, 1)},
                creatorData,
                callbackData.target
            ));

            if (ownerWeaponModel.CanSendStatus)
            {
                var targetStatusData = (IEntityStatusData)callbackData.target;
                if (targetStatusData != null)
                {
                    SimpleMessenger.Publish(MessageScope.EntityMessage, new SentStatusEffectMessage(
                        creatorData,
                        targetStatusData,
                        ownerWeaponModel.StatusIdentity
                    ));
                }
            }
        }

        protected override UniTask TriggerSpecialAttack(CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }
    }

}