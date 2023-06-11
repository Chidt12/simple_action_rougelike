using Runtime.Definition;
using System;
using System.Collections.Generic;

namespace Runtime.Gameplay.EntitySystem
{
    public interface IEntityAutoInputData : IEntityData
    {
        public AutoInputStrategyType CurrentAutoInputStrategyType { get; }
        public List<AutoInputStrategyType> AutoInputStrategyTypes { get; }
        public Action OnChangedAutoInputStrategy { get; set; }
        public void SetCurrentAutoInputStrategy(AutoInputStrategyType autoInputStrategyType);
    }
}