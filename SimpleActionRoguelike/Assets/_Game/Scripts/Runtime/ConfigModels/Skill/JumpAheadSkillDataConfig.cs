using CsvReader;
using Runtime.Gameplay.EntitySystem;
using System;

namespace Runtime.ConfigModel
{
    [Serializable]
    public class JumpAheadSkillDataConfigItem : SkillDataConfigItem
    {
        public string warningVfx;
        public float jumpDuration;
        public float jumpHeight;
        public float jumpDistance;
        public int numberOfJump;
        public float delayBetweenJump;
        public float jumpDamageBonus;
        [CsvColumnFormat(ColumnFormat = "jump_{0}")]
        public DamageFactor[] damageFactors;
    }

    public class JumpAheadSkillDataConfig : SkillDataConfig<JumpAheadSkillDataConfigItem>
    {

    }
}
