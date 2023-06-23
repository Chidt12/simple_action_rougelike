namespace Runtime.Gameplay.EntitySystem
{
    public interface IFinalDamagedModifier : IPriorityModifier
    {
        public void Finalize(float damageCreated, EffectSource effectSource, EffectProperty effectProperty, IEntityData receiver);
    }
}