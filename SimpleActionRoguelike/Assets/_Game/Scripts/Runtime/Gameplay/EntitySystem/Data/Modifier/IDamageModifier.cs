using System;

namespace Runtime.Gameplay.EntitySystem
{
    public interface IDamageModifier
    {
        int Priority { get; }
        float Damage(float value, EffectSource damageSource, EffectProperty damageProperty, IEntityData creator);
    }
}