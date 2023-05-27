using Runtime.Definition;
using System;

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
        public float delaySpawnTime;
        public bool followHero;
        public float distanceFromHero;
        public int spawnPointIndex;
    }

    [Serializable]
    public struct WaveStageLoadConfigItem
    {
        public int waveIndex;
        public int duration;
        public EntityStageLoadConfigItem[] entites;
        public bool IsInfiniteDuration => duration == -1;

        public WaveStageLoadConfigItem(int waveIndex, int duration, EntityStageLoadConfigItem[] entities)
        {
            this.waveIndex = waveIndex;
            this.duration = duration;
            this.entites = entities;
        }
    }

    [Serializable]
    public struct StageLoadConfigItem
    {
        public WaveStageLoadConfigItem[] waveConfigs;

        public StageLoadConfigItem(WaveStageLoadConfigItem[] waveConfigs)
        {
            this.waveConfigs = waveConfigs;
        }
    }
}