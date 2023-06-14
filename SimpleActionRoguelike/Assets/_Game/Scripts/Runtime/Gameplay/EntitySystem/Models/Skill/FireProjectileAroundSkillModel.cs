using Runtime.ConfigModel;
using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    public class FireProjectileAroundSkillModel : SkillModel
    {
        public override SkillType SkillType => SkillType.FireProjectileAround;

        public string ProjectileId { get; private set; }
        public int NumberOfProjectiles { get; private set; }
        public float ProjectileMoveSpeed { get; private set; }
        public float ProjectileMoveDistance { get; private set; }
        public ProjectileStrategyType ProjectileStrategyType { get; private set; }
        public float ProjectileDamageBonus { get; private set; }
        public DamageFactor[] ProjectileDamageFactors { get; private set; }
        public int WaveNumber { get; private set; }
        public float DelayBetweenWaves { get; private set; }
        public float RotateBetweenWaves { get; private set;  }

        public FireProjectileAroundSkillModel(SkillDataConfigItem configItem, bool canBeCanceled = true) : base(configItem, canBeCanceled)
        {
            var dataConfig = configItem as FireProjectileAroundSkillDataConfigItem;
            ProjectileId = dataConfig.projectileId;
            NumberOfProjectiles = dataConfig.numberOfProjectiles;
            ProjectileMoveSpeed = dataConfig.projectileMoveSpeed;
            ProjectileMoveDistance = dataConfig.projectileMoveDistance;
            ProjectileStrategyType = dataConfig.projectileStrategyType;
            ProjectileDamageBonus = dataConfig.projectileDamageBonus;
            ProjectileDamageFactors = dataConfig.projectileDamageFactors;
            WaveNumber = dataConfig.waveNumber;
            DelayBetweenWaves = dataConfig.delayBetweenWaves;
            RotateBetweenWaves = dataConfig.rotateBetweenWaves;
        }
    }
}