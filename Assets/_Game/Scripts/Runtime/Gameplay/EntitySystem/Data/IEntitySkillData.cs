using System.Collections.Generic;

namespace Runtime.Gameplay.EntitySystem
{
    public interface IEntitySkillData : IEntityData
    {
        public bool IsPlayingSkill { get; set; }
        public bool CheckCanUseSkill();
        public List<SkillModel> SkillModels { get; }
    }
}
