using Runtime.ConfigModel;
using Runtime.Definition;
using System.Collections.Generic;
using System.Linq;

namespace Runtime.Gameplay.EntitySystem
{
    public partial class EntityModel : IEntitySkillData
    {
        public List<SkillModel> _skillModels;
        public List<float> _skillDelayTimes;
        public List<TriggerPhase> _triggerPhases;
        public List<SkillModel> SkillModels => _skillModels;
        public List<float> SkillDelayTimes => _skillDelayTimes;
        public List<TriggerPhase> TriggerPhases => _triggerPhases;
        public bool IsPlayingSkill { get; set; }

        public bool CheckCanUseSkill()
        {
            return !(IsDead || IsPlayingSkill || IsDashing || currentState.IsInSkillLockedStatus());
        }

        public void InitSkills(List<SkillModel> skillModels) => _skillModels = skillModels;

        public void InitSkillDelayTimes(List<float> skillDelayTimes) => _skillDelayTimes = skillDelayTimes;

        public void InitTriggerPhases(List<TriggerPhase> triggerPhases) => _triggerPhases = triggerPhases;

        public List<int> GetSequenceSkillModelIndexes(TriggerPhase triggerPhase)
        {
            var allIndexes = new List<int>();
            for (int i = 0; i < TriggerPhases.Count; i++)
            {
                if (triggerPhase.Equals(TriggerPhases[i]))
                    allIndexes.Add(i);
            }

            return allIndexes;
        }

        /// <summary>
        /// return isFinal, and trigger phase
        /// </summary>
        /// <param name="triggerPhase"></param>
        /// <returns></returns>
        public (bool, TriggerPhase) GetNextTriggerPhase(TriggerPhase triggerPhase)
        {
            var allTriggerPhases = TriggerPhases.Distinct().ToList();
            var index = 0;
            for (int i = 0; i < allTriggerPhases.Count; i++)
            {
                if (triggerPhase.Equals(allTriggerPhases[i]))
                    index = i;
            }

            if(index < allTriggerPhases.Count - 1)
            {
                return (false, allTriggerPhases[index + 1]);
            }
            return (true, triggerPhase);
        }
    }
}
