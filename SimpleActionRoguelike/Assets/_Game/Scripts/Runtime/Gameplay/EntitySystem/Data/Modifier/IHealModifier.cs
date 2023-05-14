namespace Runtime.Gameplay.EntitySystem
{
    public interface IHealModifier
    {
        float Heal(float value, EffectSource healSource, EffectProperty healProperty, IEntityData creator);
    }
}
