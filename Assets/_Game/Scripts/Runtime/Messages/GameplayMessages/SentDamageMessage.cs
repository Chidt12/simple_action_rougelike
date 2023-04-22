using Runtime.Gameplay.EntitySystem;
using ZBase.Foundation.PubSub;

namespace Runtime.Message 
{
    public readonly struct SentDamageMessage : IMessage
    {
        public readonly EffectSource DamageSource;
        public readonly EffectProperty DamageProperty;
        public readonly float DamageBonus;
        public readonly DamageFactor[] DamageFactors;
        public readonly IEntityData Creator;
        public readonly IEntityData Target;

        public SentDamageMessage(EffectSource effectSource, EffectProperty effectProperty, float damageBonus, DamageFactor[] damageFactors, IEntityData creator, IEntityData target)
        {
            DamageSource = effectSource;
            DamageProperty = effectProperty;
            DamageBonus = damageBonus;
            DamageFactors = damageFactors;
            Creator = creator;
            Target = target;
        }
    }
}