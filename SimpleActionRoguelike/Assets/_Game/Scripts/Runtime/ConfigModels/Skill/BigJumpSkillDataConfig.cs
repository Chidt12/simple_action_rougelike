using CsvReader;
using Runtime.Gameplay.EntitySystem;
using System;

namespace Runtime.ConfigModel
{
    [Serializable]
    public class BigJumpSkillDataConfigItem : SkillDataConfigItem
    {
        public string warningVfx;
        public float displayWarningTime;
        public float jumpUpDuration;
        public float jumpMiddleDuration;
        public float jumpDownDuration;
        public float jumpHeight;
        public float jumpDistance;
        public int numberOfJump;
        public float delayBetweenJump;
        public float damageWidth;
        public float damageHeight;

        public string jumpDamageBoxPrefabName;
        public float jumpDamageBonus;
        [CsvColumnFormat(ColumnFormat = "jump_{0}")]
        public DamageFactor[] damageFactors;
    }

    public class BigJumpSkillDataConfig : SkillDataConfig<BigJumpSkillDataConfigItem>
    {
        
    }
}