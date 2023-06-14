using CsvReader;
using Runtime.Definition;
using Runtime.Gameplay.EntitySystem;
using System;

namespace Runtime.ConfigModel
{
    [Serializable]
    public class FireProjectileAroundSkillDataConfigItem : SkillDataConfigItem
    {
        public string projectileId;
        public int numberOfProjectiles;
        public float projectileMoveSpeed;
        public float projectileMoveDistance;
        public ProjectileStrategyType projectileStrategyType;
        public float projectileDamageBonus;
        [CsvColumnFormat(ColumnFormat = "projectile_{0}")]
        public DamageFactor[] projectileDamageFactors;
        public int waveNumber;
        public float delayBetweenWaves;
        public float rotateBetweenWaves;
    }

    public class FireProjectileAroundSkillDataConfig : SkillDataConfig<FireProjectileAroundSkillDataConfigItem> { }
}
