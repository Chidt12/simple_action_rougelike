using Runtime.Definition;
using System;

namespace Runtime.Gameplay.EntitySystem
{
    public static class SkillStrategyFactory
    {
        public static ISkillStrategy GetSkillStrategy(SkillType skillType)
        {
            Type elementType = Type.GetType($"Runtime.Gameplay.EntitySystem.{skillType}SkillStrategy");
            ISkillStrategy skillStrategy = Activator.CreateInstance(elementType) as ISkillStrategy;
            return skillStrategy;
        }
    }
}
