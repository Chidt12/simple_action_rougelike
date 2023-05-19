using Runtime.Definition;
using System;

namespace Runtime.Gameplay.EntitySystem
{
    public static class DeathStrategyFactory
    {
        public static IDeathStrategy GetDeathStrategy(DeathType deathType)
        {
            Type elementType = Type.GetType($"Runtime.Gameplay.EntitySystem.{deathType}DeathStrategy");
            IDeathStrategy deathStrategy = Activator.CreateInstance(elementType) as IDeathStrategy;
            return deathStrategy;
        }
    }
}
