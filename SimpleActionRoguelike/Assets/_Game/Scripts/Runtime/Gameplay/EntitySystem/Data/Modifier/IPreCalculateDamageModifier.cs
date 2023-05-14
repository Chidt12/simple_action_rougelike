namespace Runtime.Gameplay.EntitySystem
{
    public class PrepareDamageModifier
    {
        public float damageBonus;
        public float critChance;
        public EffectProperty damageProperty;
        public DamageFactor[] damageFactors;

        public PrepareDamageModifier(EffectProperty damageProperty, float damageBonus, DamageFactor[] damageFactors, float critChance)
        {
            this.damageProperty = damageProperty;
            this.damageBonus = damageBonus;
            this.damageFactors = damageFactors;
            this.critChance = critChance;
        }
    }

    public interface IPreCalculateDamageModifier
    {
        PrepareDamageModifier Calculate(IEntityData target, EffectSource damageSource, PrepareDamageModifier prepareDamageModifier);
    }
}
