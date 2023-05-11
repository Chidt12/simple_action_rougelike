using Runtime.Definition;
using System.Collections.Generic;

namespace Runtime.Gameplay.EntitySystem
{
    public partial class EntityModel : IEntitySkillData
    {
        public List<SkillModel> _skillModels;
        public List<SkillModel> SkillModels => _skillModels;
        public bool IsPlayingSkill { get; set; }

        public bool CheckCanUseSkill()
        {
            return !(IsDead || IsPlayingSkill || IsDashing || currentState.IsInSkillLockedStatus());
        }

        public void InitSkills(List<SkillModel> skillModels)
        {
            _skillModels = skillModels;
        }
    }
}
