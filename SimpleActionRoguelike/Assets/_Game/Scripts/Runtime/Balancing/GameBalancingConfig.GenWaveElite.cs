using Cysharp.Threading.Tasks;
using Runtime.ConfigModel;
using Runtime.Constants;
using Runtime.Definition;
using Runtime.Gameplay.Manager;
using Runtime.Manager.Data;
using Runtime.Manager.Gameplay;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Runtime.Gameplay.Balancing
{
    public partial class GameBalancingConfig
    {
        // Elite only have 1 wave to spawn.
        private async UniTask<(MapLevel, StageLoadConfigItem, GameplayRoomType, GameplayGateSetupType)> SetUpForEliteRoomAsync(float stagePoint, int heroLevel, GameplayGateSetupType gameplayGateSetupType, GameplayRoomType roomType, CurrentLoadedStageData currentStageData)
        {
            // Scale point.
            stagePoint = stagePoint * eliteFactor;

            // random map.
            var randomMapIndex = Random.Range(0, maps.Length);
            var randomMap = maps[randomMapIndex];
            stagePoint -= randomMap.point;

            // random enemies types.
            var possibleRandomEnemies = randomMap.includedEnemyIds != null && randomMap.includedEnemyIds.Length > 0
                                            ? randomMap.includedEnemyIds : allEliteEnemyIds.Where(x => !randomMap.exceptEnemyIds.Contains(x)).ToArray();

            var numberOfTypes = Random.Range(minEnemyTypes, Mathf.Min(maxEnemyTypes + 1, possibleRandomEnemies.Length));

            if (possibleRandomEnemies.Length <= 0)
                return (randomMap, default, GameplayRoomType.Normal, GameplayGateSetupType.None);

            List<string> selectedEnemyTypes = new List<string>();
            for (int i = 0; i < numberOfTypes; i++)
            {
                var randomList = possibleRandomEnemies.Where(x => !selectedEnemyTypes.Contains(x)).ToArray();
                var selectedIndex = Random.Range(0, randomList.Length);
                selectedEnemyTypes.Add(randomList[selectedIndex]);
            }

            // Load enemies config
            List<EnemyLevelConfigItem> selectedEnemyConfigs = new();
            foreach (var enemyId in selectedEnemyTypes)
            {
                var config = await DataManager.Config.Load<EnemyConfig>(GetConfigAssetName<EnemyConfig>(enemyId.ToString()));
                var enemyConfigItem = config.items.FirstOrDefault(x => x.id == uint.Parse(enemyId));
                var maxEnemyLevel = enemyConfigItem.levels.Max(x => x.level);
                var enemyLevel = Mathf.Min(heroLevel, maxEnemyLevel);
                var enemyLevelConfigItem = enemyConfigItem.levels.FirstOrDefault(x => x.level == enemyLevel);
                selectedEnemyConfigs.Add(enemyLevelConfigItem);
            }

            // Create waves.
            var waves = new List<WaveStageLoadConfigItem>();

            // Calculate entities for waves;
            var entities = new List<EntityStageLoadConfigItem>();
            var wavePoint = stagePoint;
            while (wavePoint > 0)
            {
                var selectedIndex = Random.Range(0, selectedEnemyConfigs.Count);
                var selectedEnemy = selectedEnemyConfigs[selectedIndex];
                var spawnedEnemyInfo = new SpawnedEntityInfo(selectedEnemyTypes[selectedIndex], EntityType.Enemy, selectedEnemy.level, 1);

                var entityStageLoadConfigItem = new EntityStageLoadConfigItem();
                entityStageLoadConfigItem.entityConfigItem = spawnedEnemyInfo;
                entityStageLoadConfigItem.delaySpawnTime = Constant.DELAY_TIME_FOR_FIRST_WAVE;
                entityStageLoadConfigItem.followHero = true;
                entityStageLoadConfigItem.distanceFromHero = randomMap.limitDistanceToHero > 0 ?
                        Mathf.Min(randomMap.limitDistanceToHero, selectedEnemy.enemyLevelStats.detectRange + 2) :
                        selectedEnemy.enemyLevelStats.detectRange + 2;

                entityStageLoadConfigItem.spawnPointIndex = 0;
                entities.Add(entityStageLoadConfigItem);

                if (selectedEnemy.point <= 0)
                    break;
                wavePoint -= selectedEnemy.point;
            }

            var newWave = new WaveStageLoadConfigItem(0, -1, entities.ToArray());
            waves.Add(newWave);

            return (randomMap, new StageLoadConfigItem(waves.ToArray()), roomType, gameplayGateSetupType);
        }
    }
}
