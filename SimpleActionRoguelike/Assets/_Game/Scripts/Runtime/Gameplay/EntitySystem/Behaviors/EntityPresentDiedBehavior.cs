using Cysharp.Threading.Tasks;
using Runtime.Core.Pool;
using System;
using System.Threading;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    [DisallowMultipleComponent]
    public class EntityPresentDiedBehavior : EntityBehavior
    {
        [SerializeField] private string _diedPrefab;
        [SerializeField] private Transform _spawnVFXPosition;

        private IEntityData _entityData;

        public async override UniTask<bool> BuildAsync(IEntityData data, CancellationToken cancellationToken)
        {
            await base.BuildAsync(data, cancellationToken);
            data.DeathEvent += OnDeath;
            _entityData = data;
            return true;
        }

        private void OnDeath()
        {
            OnDeathAsync().Forget();
        }

        private async UniTaskVoid OnDeathAsync()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(0.1f), cancellationToken: this.GetCancellationTokenOnDestroy());
            var position = _spawnVFXPosition ? _spawnVFXPosition.position : transform.position;
            PoolManager.Instance.Return(gameObject);
            if (!string.IsNullOrEmpty(_diedPrefab))
                SpawnExplodePrefabs(position).Forget();
        }

        private async UniTaskVoid SpawnExplodePrefabs(Vector2 position)
        {
            var diedGameObject = await PoolManager.Instance.Rent(_diedPrefab, token: this.GetCancellationTokenOnDestroy());
            diedGameObject.transform.position = position;
        }
    }
}
