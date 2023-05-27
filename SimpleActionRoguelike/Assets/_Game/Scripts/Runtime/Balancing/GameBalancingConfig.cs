using Cysharp.Threading.Tasks;
using Runtime.ConfigModel;
using Runtime.Constants;
using Runtime.Definition;
using Runtime.Manager.Data;
using Runtime.Manager.Gameplay;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Runtime.Gameplay.Balancing
{
    [CreateAssetMenu(fileName = AddressableKeys.GAME_BALANCING_CONFIG, menuName = "Balancing/GameBalancingConfig")]
    public class GameBalancingConfig : ScriptableObject
    {
        [HideIf(nameof(cheat))]
        [Header("=== Gen Enemies Config ===")]
        [HideIf(nameof(cheat))] public int minWave = 2; // boss only have 1 wave.
        [HideIf(nameof(cheat))] public int maxWave = 5;
        [HideIf(nameof(cheat))] public int minEnemyTypes = 1;
        [HideIf(nameof(cheat))] public int maxEnemyTypes = 4;
        [HideIf(nameof(cheat))] public int heroLevelConvert = 10;
        [HideIf(nameof(cheat))] public string[] allEnemyIds;
        [HideIf(nameof(cheat))] public string[] allBossIds;

        [HideIf(nameof(cheat))]
        [Header("=== Wave Config ===")]
        [HideIf(nameof(cheat))] public int waveTimeInSeconds = 60;

        [HideIf(nameof(cheat))]
        [Header("=== Map Config ===")]
        [HideIf(nameof(cheat))] public MapLevel lobbyMap;
        [HideIf(nameof(cheat))] public MapLevel shopMap;
        [HideIf(nameof(cheat))] public MapLevel[] maps;

        [HideIf(nameof(cheat))]
        [Header("=== Formula Balancing ===[ t*(heroPoint^n)+z ]")]
        [HideIf(nameof(cheat))] public float t;
        [HideIf(nameof(cheat))] public float n;
        [HideIf(nameof(cheat))] public float z;

        [Header("==== Cheat ====")]
        public bool cheat;
        [ShowIf(nameof(cheat))] public MapLevel cheatMap;
        [ShowIf(nameof(cheat))] public StageLoadConfigItem stageLoadConfigItem;

        #region Calculate

        public async UniTask<(MapLevel, StageLoadConfigItem, bool)> GetNextStage(int heroPoint, int stageNumber)
        {
            if (cheat)
            {
                return (cheatMap, stageLoadConfigItem, true);
            }
            else
            {
                var heroLevel = heroPoint / heroLevelConvert;
                var stagePoint = t * Mathf.Pow(heroPoint, n) + z;

                // random map.
                var randomMapIndex = Random.Range(0, maps.Length);
                var randomMap = maps[randomMapIndex];
                stagePoint -= randomMap.point;

                // random enemies types.
                var possibleRandomEnemies = randomMap.includedEnemyIds != null && randomMap.includedEnemyIds.Length > 0
                                                ? randomMap.includedEnemyIds : allEnemyIds.Where(x => !randomMap.exceptEnemyIds.Contains(x)).ToArray();

                var numberOfTypes = Random.Range(minEnemyTypes, Mathf.Min(maxEnemyTypes + 1, possibleRandomEnemies.Length));

                if (possibleRandomEnemies.Length <= 0)
                    return (randomMap, default, false);

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
                var waveNumber = Random.Range(minWave, maxWave + 1);
                var waves = new List<WaveStageLoadConfigItem>();
                for (int i = 0; i < waveNumber; i++)
                {
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
                        entityStageLoadConfigItem.delaySpawnTime = i == 0 ? 0.5f : 0;
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

                    var newWaveTime = i == waveNumber - 1 ? -1 : waveTimeInSeconds;
                    var newWave = new WaveStageLoadConfigItem(i, newWaveTime, entities.ToArray());
                    waves.Add(newWave);
                }

                return (randomMap, new StageLoadConfigItem(waves.ToArray()), true);
            }
        }

        private string GetConfigAssetName<T>(string id = "")
        {
            if (string.IsNullOrEmpty(id))
                return typeof(T).ToString();
            else
                return typeof(T).ToString() + "_" + id + ".csv";
        }

        #endregion
    }
}
