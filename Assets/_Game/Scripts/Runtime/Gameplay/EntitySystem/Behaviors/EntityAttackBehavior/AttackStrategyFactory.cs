using Runtime.Definition;
using System;

namespace Runtime.Gameplay.EntitySystem
{
    public static class AttackStrategyFactory
    {
        public static IAttackStrategy GetAttackStrategy(WeaponType weaponType)
        {
            Type elementType = Type.GetType($"Runtime.Gameplay.EntitySystem.{weaponType}AttackStrategy");
            IAttackStrategy attackStrategy = Activator.CreateInstance(elementType) as IAttackStrategy;
            return attackStrategy;
        }
    }
}
