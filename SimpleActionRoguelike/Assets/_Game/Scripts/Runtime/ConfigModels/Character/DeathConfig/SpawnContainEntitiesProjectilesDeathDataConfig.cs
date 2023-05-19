using System;
using CsvReader;
using Runtime.Gameplay.EntitySystem;

namespace Runtime.ConfigModel
{
    [Serializable]
    public class SpawnContainEntitiesProjectilesDeathDataConfigItem : DeathDataConfigItem
    {
        public uint projectileNumber;
        public string projectileId;
        public float projectileMoveSpeed;
        public float projectileMoveDistance;
        public float projectileDamageBonus;
        [CsvColumnFormat(ColumnFormat = "projectile_{0}")]
        public DamageFactor[] projectileDamageFactors;
        [CsvColumnFormat(ColumnFormat = "spawn_{0}")]
        public SpawnedEntityInfo[] spawnEntities;
        public bool useOwnerLevel;
    }

    public class SpawnContainEntitiesProjectilesDeathDataConfig : DeathDataConfig<SpawnContainEntitiesProjectilesDeathDataConfigItem>
    {
        
    }
}
