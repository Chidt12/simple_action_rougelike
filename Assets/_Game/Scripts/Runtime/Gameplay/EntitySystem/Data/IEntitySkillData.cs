using System.Collections.Generic;

namespace Runtime.Gameplay.EntitySystem
{
    public interface IEntitySkillData : IEntityData
    {
        public List<SkillModel> SkillModels { get; }
    }
}
