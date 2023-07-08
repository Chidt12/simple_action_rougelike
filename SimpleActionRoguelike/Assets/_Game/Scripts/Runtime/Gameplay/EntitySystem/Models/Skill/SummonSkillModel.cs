using Runtime.ConfigModel;
using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    public class SummonSkillModel : SkillModel
    {
        public override SkillType SkillType => SkillType.Summon;
        public SpawnedEntityInfo[] SummonedSpawnEntitiesInfo { get; private set; }
        public float SummonedCenterOffsetDistance { get; private set; }
        public bool UseOwnerLevel { get; private set; }


        public SummonSkillModel(SkillDataConfigItem configItem, int skillIndex, bool canBeCanceled = true) : base(configItem, skillIndex, canBeCanceled)
        {
            var skillConfig = configItem as SummonSkillDataConfigItem;
            SummonedSpawnEntitiesInfo = skillConfig.summonedSpawnEntitiesInfo;
            SummonedCenterOffsetDistance = skillConfig.summonedCenterOffsetDistance;
            UseOwnerLevel = skillConfig.useOwnerLevel;
        }
    }
}