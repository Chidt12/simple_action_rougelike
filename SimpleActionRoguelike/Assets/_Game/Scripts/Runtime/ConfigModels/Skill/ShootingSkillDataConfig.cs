using CsvReader;
using Runtime.Definition;
using Runtime.Gameplay.EntitySystem;
using System;

namespace Runtime.ConfigModel
{
    [Serializable]
    public class ShootingSkillDataConfigItem : SkillDataConfigItem
    {
        public string projectileId;
        public int numberOfProjectiles;
        public float delayBetweenProjectiles;
        public int numberOfBulletsInProjectile;
        public float angleBetweenBullets;

        public bool focusTargetDuringExecute;
        public float fireDeflectionAngle;

        public float projectileMoveSpeed;
        public float projectileMoveDistance;
        public ProjectileStrategyType projectileStrategyType;
        public float projectileDamageBonus;
        [CsvColumnFormat(ColumnFormat = "projectile_{0}")]
        public DamageFactor[] projectileDamageFactors;
    }

    public class ShootingSkillDataConfig : SkillDataConfig<ShootingSkillDataConfigItem>
    { 
    }
}