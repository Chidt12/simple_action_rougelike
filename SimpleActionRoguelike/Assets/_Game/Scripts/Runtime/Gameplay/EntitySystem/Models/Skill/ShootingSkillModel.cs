using Runtime.ConfigModel;
using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    public class ShootingSkillModel : SkillModel
    {
        public string ProjectileId { get; private set; }
        public int NumberOfProjectiles { get; private set; }
        public float DelayBetweenProjectiles { get; private set; }
        public float ProjectileMoveSpeed { get; private set; }
        public float ProjectileMoveDistance { get; private set; }
        public ProjectileStrategyType ProjectileStrategyType { get; private set; }
        public float ProjectileDamageBonus { get; private set; }
        public DamageFactor[] ProjectileDamageFactors { get; private set; }

        public override SkillType SkillType => SkillType.Shooting;

        public ShootingSkillModel(SkillDataConfigItem skillDataConfigItem, bool canBeCanceled = true) 
            : base(skillDataConfigItem, canBeCanceled)
        {
            var dataConfig = skillDataConfigItem as ShootingSkillDataConfigItem;
            ProjectileId = dataConfig.projectileId;
            NumberOfProjectiles = dataConfig.numberOfProjectiles;
            DelayBetweenProjectiles = dataConfig.delayBetweenProjectiles;
            ProjectileMoveSpeed = dataConfig.projectileMoveSpeed;
            ProjectileMoveDistance = dataConfig.projectileMoveDistance;
            ProjectileStrategyType = dataConfig.projectileStrategyType;
            ProjectileDamageBonus = dataConfig.projectileDamageBonus;
            ProjectileDamageFactors = dataConfig.projectileDamageFactors;
        }
    }
}