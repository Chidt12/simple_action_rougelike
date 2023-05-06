using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    public interface IStatusEffect
    {
        public StatusEffectType StatusEffecType { get; }
        public bool IsActivating { get; }
        public IEntityData Creator { get; }
        public IEntityData Owner { get; }

        public abstract void Update();
        public abstract void ForceStop();
    }
}
