using Cysharp.Threading.Tasks;
using Runtime.ConfigModel;
using Runtime.Core.Message;
using Runtime.Core.Pool;
using Runtime.Definition;
using Runtime.Message;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using ZBase.Foundation.PubSub;

namespace Runtime.Gameplay.EntitySystem
{
    public class RotateOrbsArtifactSystem : ArtifactSystem<RotateOrbsArtifactDataConfigItem>
    {
        private const string PIVOT_NAME = "weapon_pivot_point";
        private static readonly float s_angleOffset = 90f;
        private List<Projectile> _orbs;
        private Transform _weaponCenterPointTransform;
        private Transform _middleTransform;
        private int _currentOrbs;
        private CancellationTokenSource _cancellationTokenSource;
        private List<ISubscription> _subscriptions;

        public override ArtifactType ArtifactType => ArtifactType.RotateOrbs;

        public override async UniTask Init(IEntityData entityData)
        {
            await base.Init(entityData);
            _subscriptions = new();
            _subscriptions.Add(SimpleMessenger.Subscribe<EntityDiedMessage>(OnEntityDied));

            _currentOrbs = 0;
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new();

            // remove old if need.
            if (_orbs != null)
            {
                foreach (var orb in _orbs)
                    PoolManager.Instance.Return(orb.gameObject);
                _orbs.Clear();
            }
            else
            {
                _orbs = new();
            }

            _middleTransform = ownerEntityData.EntityTransform;
            var entityPositions = ownerEntityData.EntityTransform.GetComponentInChildren<EntityLocalPositions>();
            if (entityPositions)
                _middleTransform = entityPositions.Middle;


            // create new.
            _weaponCenterPointTransform = new GameObject(PIVOT_NAME).transform;

            StartRotateAsync().Forget();
        }

        private void OnEntityDied(EntityDiedMessage message)
        {
            if (message.EntityData.EntityType.IsEnemy())
            {
                if(_currentOrbs < ownerData.numberOfOrbs)
                {
                    _currentOrbs++;
                    AddProjectile().Forget();
                }
            }
        }

        public override bool CanTrigger()
        {
            return base.CanTrigger() && _orbs.Count > 0;
        }

        public override bool Trigger()
        {
            base.Trigger();
            foreach (var orb in _orbs)
            {
                
            }

            return false;
        }

        private async UniTask AddProjectile()
        {
            var projectileGameObject = await EntitiesManager.Instance.CreateProjectileAsync(ownerData.orbPrefabName, ownerEntityData, Vector2.zero, _cancellationTokenSource.Token);
            var projectile = projectileGameObject.GetComponent<Projectile>();
            IdleThroughProjectileStrategyData idleThroughProjectileStrategyData;
            idleThroughProjectileStrategyData = new IdleThroughProjectileStrategyData(false, OnGetHit);

            var projectileStrategy = ProjectileStrategyFactory.GetProjectileStrategy(ProjectileStrategyType.IdleThrough);
            projectileStrategy.Init(idleThroughProjectileStrategyData, projectile, _middleTransform.position, _middleTransform.position, default);
            projectile.InitStrategy(projectileStrategy);
            _orbs.Add(projectile);
            RearrangeSpawnedProjectiles();
        }

        private void RearrangeSpawnedProjectiles()
        {
            var angle = 360 / _orbs.Count;
            for (int i = 0; i < _orbs.Count; i++)
            {
                var spawnedProjectile = _orbs[i];
                var direction = Quaternion.AngleAxis(angle * i, Vector3.forward) * Vector2.up;
                spawnedProjectile.transform.SetParent(_weaponCenterPointTransform);
                spawnedProjectile.transform.localRotation = Quaternion.AngleAxis(angle * i + s_angleOffset, Vector3.forward);
                spawnedProjectile.transform.localPosition = direction * ownerData.flyRange;
            }
        }

        private void OnGetHit(ProjectileCallbackData callbackData)
        {
            SimpleMessenger.Publish(MessageScope.EntityMessage, new SentDamageMessage(
                EffectSource.FromArtifact,
                EffectProperty.Normal,
                ownerData.orbDamageBonus,
                ownerData.orbDamageFactors,
                ownerEntityData,
                callbackData.target
            ));
        }

        private async UniTaskVoid StartRotateAsync()
        {
            while (!ownerEntityData.IsDead)
            {
                _weaponCenterPointTransform.position = _middleTransform.position;
                _weaponCenterPointTransform.Rotate(Vector3.forward, ownerData.rotateSpeed * Time.fixedDeltaTime);
                await UniTask.WaitForFixedUpdate(_cancellationTokenSource.Token);
            }
        }

        public override void Dispose()
        {
            _cancellationTokenSource?.Cancel();
            if(_orbs != null)
            {
                foreach (var orb in _orbs)
                    PoolManager.Instance.Return(orb.gameObject);
                _orbs.Clear();
            }

            foreach (var subscription in _subscriptions)
            {
                subscription.Dispose();
            }

            if(_weaponCenterPointTransform)
                GameObject.Destroy(_weaponCenterPointTransform.gameObject);
            base.Dispose();
        }
    }
}