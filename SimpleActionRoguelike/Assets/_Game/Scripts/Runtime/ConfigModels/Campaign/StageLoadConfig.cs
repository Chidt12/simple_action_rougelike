using CsvReader;
using Runtime.Definition;
using System;
using System.Linq;

namespace Runtime.ConfigModel
{
    [Serializable]
    public struct SpawnedEntityInfo
    {
        public string entityId;
        public EntityType entityType;
        public int entityNumber;
        public int entityLevel;

        public SpawnedEntityInfo(string entityId, EntityType entityType, int entityLevel = 1, int entityNumber = 1)
        {
            this.entityId = entityId;
            this.entityType = entityType;
            this.entityNumber = entityNumber;
            this.entityLevel = entityLevel;
        }
    }

    [Serializable]
    public struct EntityStageLoadConfigItem
    {
        public SpawnedEntityInfo entityConfigItem;
        public int waveIndex;
        public float delaySpawnTime;
        public bool followHero;
        public float distanceFromHero;
        public int spawnPointIndex;
    }

    [Serializable]
    public struct WaveConfigItem
    {
        public uint stageId;
        public WaveStageLoadConfigItem[] items;
    }

    [Serializable]
    public struct WaveStageLoadConfigItem
    {
        public int waveIndex;
        public int duration;
        public bool IsInfiniteDuration => duration == -1;
    }

    [Serializable]
    public struct StageLoadConfigItem
    {
        public uint stageId;
        public EntityStageLoadConfigItem[] entites;

        [CsvColumnIgnore]
        public WaveStageLoadConfigItem[] waveConfigs;
    }

    public class StageLoadConfig : BaseConfig<StageLoadConfigItem>
    {
        private WaveConfigItem[] _tempWaveConfigs;

        public void Convert()
        {
            for (int i = 0; i < items.Length; i++)
            {
                var waveConfig = _tempWaveConfigs.FirstOrDefault(x => x.stageId == items[i].stageId);
                items[i].waveConfigs = waveConfig.items;
            }
            _tempWaveConfigs = null;
        }
    }
}