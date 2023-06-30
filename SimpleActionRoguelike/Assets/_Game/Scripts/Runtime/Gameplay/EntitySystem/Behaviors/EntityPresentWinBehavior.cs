using Cysharp.Threading.Tasks;
using Runtime.Core.Pool;
using Runtime.Definition;
using System;
using System.Threading;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem 
{

    [DisallowMultipleComponent]
    public class EntityPresentWinBehavior : EntityBehavior
    {
        [SerializeField] private string _winPrefab;
        [SerializeField] private Transform _spawnVFXPosition;
        [SerializeField] private Transform _flipTransform;

        public async override UniTask<bool> BuildAsync(IEntityData data, CancellationToken cancellationToken)
        {
            await base.BuildAsync(data, cancellationToken);
            data.ReactionChangedEvent += OnReactionChanged;
            return true;
        }

        private void OnReactionChanged(EntityReactionType reactionType)
        {
            if(reactionType == EntityReactionType.Win)
            {
                OnWinAsync().Forget();
            }
        }

        private async UniTaskVoid OnWinAsync()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(0.1f), cancellationToken: this.GetCancellationTokenOnDestroy());
            var position = _spawnVFXPosition ? _spawnVFXPosition.position : transform.position;
            if (!string.IsNullOrEmpty(_winPrefab))
            {
                SpawnWinEffect(position).Forget();
            }
        }

        private async UniTaskVoid SpawnWinEffect(Vector2 position)
        {
            var diedGameObject = await PoolManager.Instance.Rent(_winPrefab, token: this.GetCancellationTokenOnDestroy());
            diedGameObject.transform.position = position;
            if (_flipTransform)
                diedGameObject.transform.localScale = _flipTransform.localScale;
            PoolManager.Instance.Return(gameObject);
        }
    }
}