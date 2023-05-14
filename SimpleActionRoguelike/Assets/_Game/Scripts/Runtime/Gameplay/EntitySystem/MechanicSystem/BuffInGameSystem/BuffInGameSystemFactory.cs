using Runtime.Definition;
using System;

namespace Runtime.Gameplay.EntitySystem
{
    public static class BuffInGameSystemFactory
    {
        public static IBuffInGameSystem GetBuffInGameSystem(BuffInGameType buffInGameType)
        {
            Type elementType = Type.GetType($"Runtime.Gameplay.EntitySystem.{buffInGameType}BuffInGameSystem");
            IBuffInGameSystem buffInGameSystem = Activator.CreateInstance(elementType) as IBuffInGameSystem;
            return buffInGameSystem;
        }
    }
}
