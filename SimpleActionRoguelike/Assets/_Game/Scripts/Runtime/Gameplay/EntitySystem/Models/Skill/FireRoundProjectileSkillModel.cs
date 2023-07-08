using Runtime.ConfigModel;
using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    public class FireRoundProjectileSkillModel : SkillModel
    {
        public override SkillType SkillType => SkillType.FireRoundProjectile;
        public string ProjectileId { get; private set; }
        public string ImpactPrefabName { get; private set; }
        public int NumberOfProjectiles { get; private set; }
        public int NumberOfBulletsInProjectile { get; private set; }
        public float AngleBetweenBullet { get; private set; }
        public float DelayBetweenProjectile { get; private set; }
        
        public float MaxProjectileFlyDistance { get; private set; }
        public bool FocusTargetDuringExecute { get; private set; }
        public float ProjectileFlyDuration { get; private set; }
        public float ProjectileFlyHeight { get; private set; }
        public string WarningPrefabName { get; private set; }
        public float DamageAreaHeight { get; private set; }
        public float DamageAreaWidth { get; private set; }
        public DamageFactor[] DamageAreaDamageFactors { get; private set; }
        public float DamageAreaLifeTime { get; private set; }
        public float DamageAreaInterval { get; private set; }
        public string DamageAreaPrefabName { get; private set; }
        public DamageFactor[] FirstInitDamageFactors { get; private set; }

        public FireRoundProjectileSkillModel(SkillDataConfigItem configItem, int skillIndex, bool canBeCanceled = true) : base(configItem, skillIndex, canBeCanceled)
        {
            var skillConfig = configItem as FireRoundProjectileSkillDataConfigItem;
            ProjectileId = skillConfig.projectileId;
            ImpactPrefabName = skillConfig.impactPrefabName;

            NumberOfBulletsInProjectile = skillConfig.numberOfBulletsInProjectile;
            NumberOfProjectiles = skillConfig.numberOfProjectiles;
            AngleBetweenBullet = skillConfig.angleBetweenBullet;
            DelayBetweenProjectile = skillConfig.delayBetweenProjectile;
            MaxProjectileFlyDistance = skillConfig.maxProjectileFlyDistance;
            FocusTargetDuringExecute = skillConfig.focusTargetDuringExecute;

            ProjectileFlyDuration = skillConfig.projectileFlyDuration;
            ProjectileFlyHeight = skillConfig.projectileFlyHeight;
            WarningPrefabName = skillConfig.warningPrefabName;
            DamageAreaHeight = skillConfig.damageAreaHeight;
            DamageAreaWidth = skillConfig.damageAreaWidth;
            DamageAreaDamageFactors = skillConfig.damageAreaDamageFactors;
            DamageAreaLifeTime = skillConfig.damageAreaLifeTime;
            DamageAreaInterval = skillConfig.damageAreaInterval;
            DamageAreaPrefabName = skillConfig.damageAreaPrefabName;
            FirstInitDamageFactors = skillConfig.firstInitDamageFactors;
        }
    }
}