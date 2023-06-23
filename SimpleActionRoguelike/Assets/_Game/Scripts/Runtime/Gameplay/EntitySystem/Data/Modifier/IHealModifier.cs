namespace Runtime.Gameplay.EntitySystem
{
    public interface IHealModifier : IPriorityModifier
    {
        float Heal(float value, EffectSource healSource, EffectProperty healProperty, IEntityData creator);
    }
}
