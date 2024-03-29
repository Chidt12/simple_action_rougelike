using Runtime.ConfigModel;
using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    public abstract class SkillModel
    {
        public float CastRange { get; protected set; }
        public bool IsReady => CurrentSkillPhase == SkillPhase.Ready;
        public virtual bool DependTarget { get; protected set; }
        public SkillTargetType TargetType { get; protected set; }
        public bool CanBeCanceled { get; protected set; }
        public abstract SkillType SkillType { get; }
        public SkillPhase CurrentSkillPhase {get; set;}
        public int SkillIndex { get; protected set; }

        public SkillModel(SkillDataConfigItem configItem, int skillIndex, bool canBeCanceled = true)
        {
            TargetType = configItem.targetType;
            CastRange = configItem.castRange;
            DependTarget = configItem.dependTarget;
            CanBeCanceled = canBeCanceled;
            SkillIndex = skillIndex;
        }
    }

    public class SkillData
    {
        #region Members

        public SkillDataConfigItem configItem;
        public int skillIndex;

        #endregion Members

        #region Class Methods

        public SkillData(SkillDataConfigItem configItem, int skillIndex)
        {
            this.configItem = configItem;
            this.skillIndex = skillIndex;
        }

        #endregion Class Methods
    }
}
