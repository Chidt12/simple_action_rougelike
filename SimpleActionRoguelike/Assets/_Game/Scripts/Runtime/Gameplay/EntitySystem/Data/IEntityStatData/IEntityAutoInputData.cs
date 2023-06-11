using Runtime.Definition;
using System;

namespace Runtime.Gameplay.EntitySystem
{
    public interface IEntityAutoInputData : IEntityData
    {
        public abstract AutoInputStrategyType CurrentAutoInputStrategyType { get; }
        public Action OnChangedAutoInputStrategy { get; set; }
        public void SetCurrentAutoInputStrategy(int index);
    }
}