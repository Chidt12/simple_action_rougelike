using CsvReader;
using Runtime.Gameplay.EntitySystem;
using System;

namespace Runtime.ConfigModel
{
    [Serializable]
    public class FireRoundProjectileSkillDataConfigItem : SkillDataConfigItem
    {
        public string projectileId;
        public string impactPrefabName;
        public int numberOfProjectiles;
        public int numberOfBulletsInProjectile;
        public float angleBetweenBullet;
        public float delayBetweenProjectile;
        public float maxProjectileFlyDistance;
        public bool focusTargetDuringExecute;
        public float projectileFlyDuration;
        public float projectileFlyHeight;
        public string warningPrefabName;
        public float damageAreaHeight;
        public float damageAreaWidth;
        [CsvColumnFormat(ColumnFormat = "damage_area_{0}")]
        public DamageFactor[] damageAreaDamageFactors;
        public float damageAreaLifeTime;
        public float damageAreaInterval;
        public string damageAreaPrefabName;
        [CsvColumnFormat(ColumnFormat = "first_init_{0}")]
        public DamageFactor[] firstInitDamageFactors;
    }

    public class FireRoundProjectileSkillDataConfig : SkillDataConfig<FireRoundProjectileSkillDataConfigItem>
    {
    }
}