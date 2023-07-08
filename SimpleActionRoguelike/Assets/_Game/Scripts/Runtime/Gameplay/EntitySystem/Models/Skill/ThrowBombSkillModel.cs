using Runtime.ConfigModel;
using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    public class ThrowBombSkillModel : SkillModel
    {
        public override SkillType SkillType => SkillType.ThrowBomb;

        public int NumberOfBombs { get; private set; }
        public float DelayBetweenBombs { get; private set; }
        public float WarningTime { get; private set; }
        public string WarningPrefabName { get; private set; }
        public string ImpactPrefabName { get; private set; }
        public float ImpactWidth { get; private set; }
        public float ImpactHeight { get; private set; }
        public float DamageBonus { get; private set; }
        public DamageFactor[] DamageFactors { get; private set; }
        public bool Random;
        public float OffsetRandom;

        public ThrowBombSkillModel(SkillDataConfigItem configItem, int skillIndex, bool canBeCanceled = true) : base(configItem, skillIndex, canBeCanceled)
        {
            var skillConfig = configItem as ThrowBombSkillDataConfigItem;
            NumberOfBombs = skillConfig.numberOfBombs;
            DelayBetweenBombs = skillConfig.delayBetweenBombs;
            WarningTime = skillConfig.warningTime;
            WarningPrefabName = skillConfig.warningPrefabName;
            ImpactPrefabName = skillConfig.impactPrefabName;
            ImpactWidth = skillConfig.impactWidth;
            ImpactHeight = skillConfig.impactHeight;
            DamageBonus = skillConfig.damageBonus;
            DamageFactors = skillConfig.damageFactors;
            Random = skillConfig.random;
            OffsetRandom = skillConfig.offsetRandom;
        }
    }
}