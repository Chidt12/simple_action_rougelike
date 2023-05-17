using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    public interface IEntityAutoInputData : IEntityData
    {
        public abstract AutoInputStrategyType AutoInputStrategyType { get; }
    }
}