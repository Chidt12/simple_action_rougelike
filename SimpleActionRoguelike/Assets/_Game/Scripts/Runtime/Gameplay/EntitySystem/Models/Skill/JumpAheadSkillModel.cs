using Runtime.ConfigModel;
using Runtime.Definition;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public class JumpAheadSkillModel : SkillModel
    {
        public override SkillType SkillType => SkillType.JumpAhead;
        public string WarningVfx { get; private set; }
        public float JumpHeight { get; private set; }
        public int NumberOfJump { get; private set; }
        public float JumpDistance { get; private set; }
        public float DelayBetweenJump { get; private set; }
        public float JumpDuration { get; private set; }
        public float JumpDamageBonus { get; private set; }
        public DamageFactor[] JumDamageFactors { get; private set; }

        public JumpAheadSkillModel(SkillDataConfigItem configItem, int skillIndex, bool canBeCanceled = true) 
            : base(configItem, skillIndex, canBeCanceled)
        {
            var dataConfig = configItem as JumpAheadSkillDataConfigItem;
            WarningVfx = dataConfig.warningVfx;
            JumpHeight = dataConfig.jumpHeight;
            NumberOfJump = dataConfig.numberOfJump;
            JumpDistance = dataConfig.jumpDistance;
            DelayBetweenJump = dataConfig.delayBetweenJump;
            JumpDamageBonus = dataConfig.jumpDamageBonus;
            JumDamageFactors = dataConfig.damageFactors;
            JumpDuration = dataConfig.jumpDuration;
        }
    }
}