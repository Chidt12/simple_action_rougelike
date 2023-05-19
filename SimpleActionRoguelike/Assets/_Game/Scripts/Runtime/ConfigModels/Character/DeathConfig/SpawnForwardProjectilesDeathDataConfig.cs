using System;
using CsvReader;
using Runtime.Gameplay.EntitySystem;

namespace Runtime.ConfigModel
{
    [Serializable]
    public class SpawnForwardProjectilesDeathDataConfigItem : DeathDataConfigItem
    {
        public uint projectileNumber;
        public string projectileId;
        public float projectileMoveSpeed;
        public float projectileMoveDistance;
        public float projectileDamageBonus;
        [CsvColumnFormat(ColumnFormat = "projectile_{0}")]
        public DamageFactor[] projectileDamageFactors;
    }

    public class SpawnForwardProjectilesDeathDataConfig : DeathDataConfig<SpawnForwardProjectilesDeathDataConfigItem>
    {
    }
}
