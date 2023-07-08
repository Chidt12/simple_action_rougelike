using Runtime.ConfigModel;
using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    public class BigJumpSkillModel : SkillModel
    {
        public override SkillType SkillType => SkillType.BigJump;
        public string WarningVfx { get; private set; }
        public float DisplayWarningTime { get; private set; }
        public float JumpHeight { get; private set; }
        public int NumberOfJump { get; private set; }
        public float JumpDistance { get; private set; }
        public float DelayBetweenJump { get; private set; }
        public float JumpUpDuration { get; private set; }
        public float JumpMiddleDuration { get; private set; }
        public string JumpDamageBoxPrefabName { get; private set; }
        public float JumpDownDuration { get; private set; }
        public float JumpDamageBonus { get; private set; }
        public float DamageWidth { get; private set; }
        public float DamageHeight { get; private set; }
        public DamageFactor[] JumDamageFactors { get; private set; }

        public BigJumpSkillModel(SkillDataConfigItem configItem, int skillIndex, bool canBeCanceled = true) : base(configItem, skillIndex, canBeCanceled)
        {
            var dataConfig = configItem as BigJumpSkillDataConfigItem;
            WarningVfx = dataConfig.warningVfx;
            DisplayWarningTime = dataConfig.displayWarningTime;
            JumpHeight = dataConfig.jumpHeight;
            NumberOfJump = dataConfig.numberOfJump;
            JumpDistance = dataConfig.jumpDistance;
            DelayBetweenJump = dataConfig.delayBetweenJump;
            JumpDamageBonus = dataConfig.jumpDamageBonus;
            JumDamageFactors = dataConfig.damageFactors;
            JumpUpDuration = dataConfig.jumpUpDuration;
            JumpMiddleDuration = dataConfig.jumpMiddleDuration;
            JumpDownDuration = dataConfig.jumpDownDuration;
            DamageWidth = dataConfig.damageWidth;
            DamageHeight = dataConfig.damageHeight;
            JumpDamageBoxPrefabName = dataConfig.jumpDamageBoxPrefabName;
        }
    }
}