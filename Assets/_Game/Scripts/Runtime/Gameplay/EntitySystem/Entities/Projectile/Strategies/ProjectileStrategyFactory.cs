using Runtime.Definition;
using System;

namespace Runtime.Gameplay.EntitySystem
{
    public static class ProjectileStrategyFactory
    {
        public static IProjectileStrategy GetProjectilStrategy(ProjectileStrategyType projectileStrategyType)
        {
            Type elementType = Type.GetType($"Runtime.Gameplay.EntitySystem.{projectileStrategyType}ProjectileStrategy");
            IProjectileStrategy projectileStrategy = Activator.CreateInstance(elementType) as IProjectileStrategy;
            return projectileStrategy;
        }
    }
}