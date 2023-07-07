using Runtime.ConfigModel;
using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    public class RushAttackSkillModel : SkillModel
    {
        public override SkillType SkillType => SkillType.RushAttack;

        public float RushWidth { get; private set; }
        public float RushRange { get; private set; }
        public float RushDuration { get; private set; }
        public float WarningRushDuration { get; private set; }
        public int NumberOfRushTime { get; private set; }
        public float DelayBetweenRush { get; private set; }
        public bool StopRushingAfterHitTarget { get; private set; }
        public float RushDamageBonus { get; private set; }
        public DamageFactor[] RushDamageFactors { get; private set; }

        public RushAttackSkillModel(SkillDataConfigItem configItem, int skillIndex, bool canBeCanceled = true) : base(configItem, skillIndex, canBeCanceled)
        {
            var dataConfig = configItem as RushAttackSkillDataConfigItem;
            RushWidth = dataConfig.rushWidth;
            RushRange = dataConfig.rushRange;
            RushDuration = dataConfig.rushDuration;
            WarningRushDuration = dataConfig.warningRushDuration;
            NumberOfRushTime = dataConfig.numberOfRushTime;
            StopRushingAfterHitTarget = dataConfig.stopRushingAfterHitTarget;
            RushDamageBonus = dataConfig.rushDamageBonus;
            RushDamageFactors = dataConfig.rushDamageFactors;
            DelayBetweenRush = dataConfig.delayBetweenRush;
        }
    }
}