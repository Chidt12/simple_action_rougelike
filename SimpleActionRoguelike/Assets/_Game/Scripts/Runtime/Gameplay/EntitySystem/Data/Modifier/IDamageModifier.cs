namespace Runtime.Gameplay.EntitySystem
{
    public interface IDamageModifier
    {
        float Damage(float value, EffectSource damageSource, EffectProperty damageProperty, IEntityData creator);
    }
}