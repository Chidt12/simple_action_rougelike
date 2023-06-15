using Runtime.Definition;
using System;

namespace Runtime.Gameplay.EntitySystem
{
    public static class SkillModelFactory
    {
        public static SkillModel GetSkillModel(SkillType skillType, SkillData skillData)
        {
            Type elementType = Type.GetType($"Runtime.Gameplay.EntitySystem.{skillType}SkillModel");
            SkillModel skillModel = Activator.CreateInstance(elementType, skillData.configItem, skillData.skillIndex, skillData.configItem.canBeCanceled) as SkillModel;
            return skillModel;
        }
    }
}