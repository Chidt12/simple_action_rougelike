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
        // Boss Only Have 1 Wave to spawn.
        private async UniTask<(MapLevelScriptableObject, StageLoadConfigItem, GameplayRoomType, GameplayGateSetupType)> SetUpForBossRoomAsync(float stagePoint, int heroLevel, GameplayGateSetupType gameplayGateSetupType, GameplayRoomType roomType, CurrentLoadedStageData currentStageData)
        {
            // Update stage points
            stagePoint = stagePoint * bossFactor;

            // random map.
            var randomMapIndex = Random.Range(0, bossMaps.Length);
            var randomMap = bossMaps[randomMapIndex];
            stagePoint -= randomMap.point;

            // Create waves.
            var waves = new List<WaveStageLoadConfigItem>();

            // random enemies types.

            var foundBossId = false;
            var bossId = string.Empty;
            if(randomMap.includedEnemyIds != null && randomMap.includedEnemyIds.Length > 0)
            {
                var possibleBossIds = randomMap.includedEnemyIds.Where(x => allBossIds.Contains(x)).ToList();
                if(possibleBossIds.Count > 0)
                {
                    bossId = possibleBossIds[Random.Range(0, possibleBossIds.Count)];
                    foundBossId = true;
                }
            }

            if (!foundBossId)
            {
                var possibleBossIds = allBossIds.Where(x => !randomMap.exceptEnemyIds.Contains(x)).ToArray();
                bossId = possibleBossIds[Random.Range(0, possibleBossIds.Length)];
            }

            // Spawn Boss First
            var entities = new List<EntityStageLoadConfigItem>();
            var wavePoint = stagePoint;

            var bossConfig = await DataManager.Config.Load<EnemyConfig>(GetConfigAssetName<EnemyConfig>(bossId));
            var bossConfigItem = bossConfig.items.FirstOrDefault(x => x.id == uint.Parse(bossId));
            var maxBossLevel = bossConfigItem.levels.Max(x => x.level);
            var bossLevel = Mathf.Min(heroLevel, maxBossLevel);
            var bossLevelConfigItem = bossConfigItem.levels.FirstOrDefault(x => x.level == bossLevel);
            var spawnedBossInfo = new SpawnedEntityInfo(bossId, EntityType.Boss, bossLevelConfigItem.level, 1);

            var bossStageLoadConfigItem = new EntityStageLoadConfigItem();
            bossStageLoadConfigItem.entityConfigItem = spawnedBossInfo;
            bossStageLoadConfigItem.delaySpawnTime = Constant.DELAY_TIME_FOR_FIRST_WAVE;
            bossStageLoadConfigItem.followHero = false;
            bossStageLoadConfigItem.spawnPointIndex = 1;

            entities.Add(bossStageLoadConfigItem);
            wavePoint -= bossLevelConfigItem.point;

            // Calculate entities for waves if boss is not strong enough.

            if (wavePoint > 0)
            {
                var possibleRandomEnemies = randomMap.includedEnemyIds != null && randomMap.includedEnemyIds.Length > 0
                                         ? randomMap.includedEnemyIds : allEnemyIds.Where(x => !randomMap.exceptEnemyIds.Contains(x)).ToArray();

                var numberOfTypes = Random.Range(minEnemyTypes, Mathf.Min(maxEnemyTypes, possibleRandomEnemies.Length));

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
            }

            var newWave = new WaveStageLoadConfigItem(0, -1, entities.ToArray());
            waves.Add(newWave);

            return (randomMap, new StageLoadConfigItem(waves.ToArray()), roomType, gameplayGateSetupType);
        }
    }
}
