using System;

namespace Runtime.Gameplay.EntitySystem
{
    public interface IPriorityModifier
    {
        /// <summary>
        /// Lowest is run first, highest is run last
        /// </summary>
        int Priority { get; }
    }

    public interface IDamageModifier : IPriorityModifier
    {
        float Damage(float value, EffectSource damageSource, EffectProperty damageProperty, IEntityData creator);
    }
}