using Runtime.Definition;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Runtime.Gameplay.EntitySystem
{
    public partial class EntityModel : IEntityAutoInputData
    {
        protected AutoInputStrategyType currentAutoInputStrategyType;
        protected List<AutoInputStrategyType> autoInputStrategyTypes;
        public List<AutoInputStrategyType> AutoInputStrategyTypes => autoInputStrategyTypes;

        public AutoInputStrategyType CurrentAutoInputStrategyType => currentAutoInputStrategyType;

        public Action OnChangedAutoInputStrategy { get; set; }

        public void InitAutoInputStrategy(List<AutoInputStrategyType> autoInputStrategyTypes)
        {
            this.autoInputStrategyTypes = autoInputStrategyTypes;
            currentAutoInputStrategyType = autoInputStrategyTypes.FirstOrDefault();
        }

        public void SetCurrentAutoInputStrategy(AutoInputStrategyType autoInputStrategyType)
        {
            currentAutoInputStrategyType = autoInputStrategyType;
            OnChangedAutoInputStrategy?.Invoke();
        }
    }
}
