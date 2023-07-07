using CsvReader;
using Runtime.Gameplay.EntitySystem;
using System;

namespace Runtime.ConfigModel
{
    [Serializable]
    public class RushAttackSkillDataConfigItem : SkillDataConfigItem
    {
        public float rushWidth;
        public float rushRange;
        public float rushDuration;
        public float warningRushDuration;
        public int numberOfRushTime;
        public float delayBetweenRush;
        public bool stopRushingAfterHitTarget;
        public float rushDamageBonus;
        [CsvColumnFormat(ColumnFormat = "rush_{0}")]
        public DamageFactor[] rushDamageFactors;
    }

    public class RushAttackSkillDataConfig : SkillDataConfig<RushAttackSkillDataConfigItem>
    {
    }
}