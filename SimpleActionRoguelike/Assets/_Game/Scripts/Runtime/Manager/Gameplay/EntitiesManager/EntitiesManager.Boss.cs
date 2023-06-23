using Cysharp.Threading.Tasks;
using Runtime.Core.Message;
using Runtime.Core.Pool;
using Runtime.Definition;
using Runtime.Manager.Gameplay;
using Runtime.Message;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public partial class EntitiesManager
    {
        private async UniTask<GameObject> CreateBossAsync(int entityId, int entityLevel, Vector2 spawnPosition, CancellationToken cancellationToken = default)
        {
            var enemyGameObject = await PoolManager.Instance.Rent(entityId.ToString(), isActive: false, token: cancellationToken);
            enemyGameObject = await LoadBossAsync(enemyGameObject, entityId, entityLevel, spawnPosition, cancellationToken);
            return enemyGameObject;
        }

        private async UniTask<GameObject> LoadBossOnMapAsync(GameObject entityGameObject, int entityId, int entityLevel, CancellationToken cancellationToken = default)
        {
            entityGameObject.SetActive(false);
            entityGameObject = await LoadBossAsync(entityGameObject, entityId, entityLevel, entityGameObject.transform.position, cancellationToken);
            return entityGameObject;
        }

        private async UniTask<GameObject> LoadBossAsync(GameObject enemyGameObject, int entityId, int entityLevel, Vector2 spawnPosition, CancellationToken cancellationToken = default)
        {
            var bossData = await GameplayDataManager.Instance.GetBossDataAsync(entityId, entityLevel);
            var bossModel = new BossModel();
            bossModel.Init(EntityType.Boss, _entityUId++, entityId, entityLevel);
            bossModel.InitStats(bossData.Item1);

            bossModel.InitSkills(bossData.Item2);

            var delayTimes = bossData.Item3.skillDelayTimes.Select(x => x.value).ToList();
            bossModel.InitSkillDelayTimes(delayTimes);
            bossModel.InitTriggerPhases(bossData.Item3.skillTriggerPhases.ToList());

            bossModel.InitStatus();
            bossModel.InitAutoInputStrategy(bossData.Item3.autoInputStrategies.ToList());
            bossModel.InitDeathData(bossData.Item3.deathDataIdentity);

            enemyGameObject.SetActive(true);
            enemyGameObject.transform.position = spawnPosition;
            await enemyGameObject.GetComponent<IEntityHolder>().BuildAsync(bossModel);

            SimpleMessenger.Publish(new EntitySpawnedMessage(bossModel, enemyGameObject.transform));
            EnemiesData.Add(bossModel);

            CreateEntitySpawnVfxAsync(bossModel.EntityType, spawnPosition, cancellationToken).Forget();

            return enemyGameObject;
        }
    }
}
