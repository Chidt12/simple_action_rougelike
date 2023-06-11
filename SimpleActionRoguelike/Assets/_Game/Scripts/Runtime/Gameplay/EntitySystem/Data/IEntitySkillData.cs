using Runtime.ConfigModel;
using System.Collections.Generic;

namespace Runtime.Gameplay.EntitySystem
{
    public interface IEntitySkillData : IEntityData
    {
        public bool IsPlayingSkill { get; set; }
        public bool CheckCanUseSkill();
        public List<SkillModel> SkillModels { get; }
        public List<float> SkillDelayTimes { get; }
        public List<TriggerPhase> TriggerPhases { get; }

        public List<int> GetSequenceSkillModelIndexes(TriggerPhase triggerPhase);
        public TriggerPhase GetNextTriggerPhase(TriggerPhase triggerPhase);
    }
}
