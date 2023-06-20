using Cysharp.Threading.Tasks;
using Runtime.Core.Message;
using Runtime.Core.Pool;
using Runtime.Definition;
using Runtime.Message;
using System.Threading;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public partial class EntitiesManager
    {
        private async UniTask<GameObject> CreateAssetAsync(int entityId, int entityLevel, Vector2 spawnPosition, CancellationToken cancellationToken = default)
        {
            var enemyGameObject = await PoolManager.Instance.Rent(entityId.ToString(), isActive: false, token: cancellationToken);
            enemyGameObject = await LoadAssetAsync(enemyGameObject, entityId, entityLevel, spawnPosition, cancellationToken);
            return enemyGameObject;
        }

        private async UniTask<GameObject> LoadAssetOnMapAsync(GameObject entityGameObject, int entityId, int entityLevel, CancellationToken cancellationToken = default)
        {
            entityGameObject.SetActive(false);
            entityGameObject = await LoadAssetAsync(entityGameObject, entityId, entityLevel, entityGameObject.transform.position, cancellationToken);
            return entityGameObject;
        }

        private async UniTask<GameObject> LoadAssetAsync(GameObject enemyGameObject, int entityId, int entityLevel, Vector2 spawnPosition, CancellationToken cancellationToken = default)
        {
            var assetData = await GameplayDataManager.Instance.GetAssetDataAsync(entityId, entityLevel);
            var assetModel = new AssetModel();
            assetModel.Init(EntityType.Asset, _entityUId++, entityId, entityLevel);
            assetModel.InitStats(assetData.Item1);

            enemyGameObject.SetActive(true);
            enemyGameObject.transform.position = spawnPosition;
            await enemyGameObject.GetComponent<IEntityHolder>().BuildAsync(assetModel);

            SimpleMessenger.Publish(new EntitySpawnedMessage(assetModel, enemyGameObject.transform));

            return enemyGameObject;
        }
    }

}