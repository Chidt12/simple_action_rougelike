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
        private async UniTask<GameObject> CreateHeroAsync(int entityId, int entityLevel, Vector2 spawnPosition, CancellationToken cancellationToken = default)
        {
            var heroGameObject = await PoolManager.Instance.Rent(entityId.ToString(), token: cancellationToken);
            heroGameObject = await LoadHeroAsync(heroGameObject, entityId, entityLevel, spawnPosition, cancellationToken);
            return heroGameObject;
        }

        private async UniTask<GameObject> LoadHeroOnMapAsync(GameObject entityGameObject, int entityId, int entityLevel, CancellationToken cancellationToken = default)
        {
            entityGameObject = await LoadHeroAsync(entityGameObject, entityId, entityLevel, entityGameObject.transform.position, cancellationToken);
            return entityGameObject;
        }

        private async UniTask<GameObject> LoadHeroAsync(GameObject entityGameObject, int entityId, int entityLevel, Vector2 spawnPosition, CancellationToken cancellationToken = default)
        {
            var heroData = await GameplayDataManager.Instance.GetHeroDataAsync(entityId);
            var heroModel = new HeroModel();
            heroModel.Init(EntityType.Hero, _entityUId++, entityId, GameplayDataDispatcher.Instance.HeroLevel);
            heroModel.InitStats(heroData.Item1);
            heroModel.InitWeapon(heroData.Item2);
            heroModel.InitStatus();
            entityGameObject.SetActive(true);
            entityGameObject.transform.position = spawnPosition;
            await entityGameObject.GetComponent<IEntityHolder>().BuildAsync(heroModel);
            SimpleMessenger.Publish(new HeroSpawnedMessage(heroModel, entityGameObject.transform));
            HeroData = heroModel;

            CreateEntitySpawnVfxAsync(heroModel.EntityType, spawnPosition, cancellationToken).Forget();
            return entityGameObject;
        }
    }

}