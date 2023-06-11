using Cysharp.Threading.Tasks;
using Runtime.Core.Message;
using Runtime.Core.Pool;
using Runtime.Definition;
using Runtime.Message;
using System;
using System.Threading;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public partial class EntitiesManager
    {
        private async UniTask<GameObject> CreateEnemyAsync(int entityId, int entityLevel, Vector2 spawnPosition, CancellationToken cancellationToken = default)
        {
            var enemyGameObject = await PoolManager.Instance.Rent(entityId.ToString(), isActive: false, token: cancellationToken);
            enemyGameObject = await LoadEnemyAsync(enemyGameObject, entityId, entityLevel, spawnPosition, cancellationToken);
            return enemyGameObject;
        }

        private async UniTask<GameObject> LoadEnemyOnMapAsync(GameObject entityGameObject, int entityId, int entityLevel, CancellationToken cancellationToken = default)
        {
            entityGameObject.SetActive(false);
            entityGameObject = await LoadEnemyAsync(entityGameObject, entityId, entityLevel, entityGameObject.transform.position, cancellationToken);
            return entityGameObject;
        }

        private async UniTask<GameObject> LoadEnemyAsync(GameObject enemyGameObject, int entityId, int entityLevel, Vector2 spawnPosition, CancellationToken cancellationToken = default)
        {
            var enemyData = await GameplayDataManager.Instance.GetEnemyDataAsync(entityId, entityLevel);
            var enemyModel = new EnemyModel();
            enemyModel.Init(EntityType.Enemy, _entityUId++, entityId, entityLevel);
            enemyModel.InitStats(enemyData.Item1);
            enemyModel.InitSkills(enemyData.Item2);
            enemyModel.InitStatus();
            enemyModel.InitAutoInputStrategy(new() { enemyData.Item3.autoInputStrategy });
            enemyModel.InitDeathData(enemyData.Item3.deathDataIdentity);

            enemyGameObject.SetActive(true);
            enemyGameObject.transform.position = spawnPosition;
            await enemyGameObject.GetComponent<IEntityHolder>().BuildAsync(enemyModel);

            SimpleMessenger.Publish(new EntitySpawnedMessage(enemyModel, enemyGameObject.transform));
            EnemiesData.Add(enemyModel);

            CreateEntitySpawnVfxAsync(enemyModel.EntityType, spawnPosition, cancellationToken).Forget();

            return enemyGameObject;
        }
    }
}
