using Runtime.ConfigModel;
using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    public abstract class SkillModel
    {
        public float CastRange { get; protected set; }
        public float Cooldown { get; set; }
        public float CurrentCooldown { get; set; }
        public bool IsReady => CurrentSkillPhase == SkillPhase.Ready;
        public bool DependTarget { get; protected set; }
        public SkillTargetType TargetType { get; protected set; }
        public bool CanBeCanceled { get; protected set; }
        public abstract SkillType SkillType { get; }
        public SkillPhase CurrentSkillPhase {get; set;}

        public SkillModel(SkillDataConfigItem configItem, bool canBeCanceled = true)
        {
            TargetType = configItem.targetType;
            CastRange = configItem.castRange;
            Cooldown = configItem.cooldown;
            DependTarget = configItem.dependTarget;
            CanBeCanceled = canBeCanceled;
        }
    }

    public class SkillData
    {
        #region Members

        public SkillDataConfigItem configItem;

        #endregion Members

        #region Class Methods

        public SkillData(SkillDataConfigItem configItem)
        {
            this.configItem = configItem;
        }

        #endregion Class Methods
    }
}
