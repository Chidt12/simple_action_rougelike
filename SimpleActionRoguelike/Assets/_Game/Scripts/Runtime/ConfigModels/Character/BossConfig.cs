using CsvReader;
using Runtime.Definition;
using System;

namespace Runtime.ConfigModel
{
    public enum TriggerPhaseType
    {
        Normal = 0,
        HealthDecrease,
        EntityKilled,
    }

    [Serializable]
    public class BossLevelStats : CharacterLevelStats
    {
        public float collideDamage;
    }

    public struct CustomFloat
    {
        public float value;
    }

    public struct TriggerPhase
    {
        public TriggerPhaseType triggerPhaseType;
        public float triggerPhaseHealth;
        public int triggerPhaseEntityId;
    }


    [Serializable]
    public class BossLevelConfigItem : CharacterLevelConfigItem
    {
        public BossLevelStats bossLevelStats;
        public DeathDataIdentity deathDataIdentity;

        public SkillIdentity[] skillIdentities;
        [CsvColumnFormat(ColumnFormat = "skill_delay_time")]
        public CustomFloat[] skillDelayTimes;
        public TriggerPhase[] skillTriggerPhases;
        public AutoInputStrategyType[] autoInputStrategies;

        public override CharacterLevelStats CharacterLevelStats => bossLevelStats;
    }

    [Serializable]
    public class BossConfigItem : CharacterConfigItem<BossLevelConfigItem> { }

    public class BossConfig : BaseConfig<BossConfigItem> {}
}
