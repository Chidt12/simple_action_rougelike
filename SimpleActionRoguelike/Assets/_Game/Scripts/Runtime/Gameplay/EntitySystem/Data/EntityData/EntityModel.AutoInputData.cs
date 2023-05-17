using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    public partial class EntityModel : IEntityAutoInputData
    {
        protected AutoInputStrategyType autoInputStrategyType;
        public AutoInputStrategyType AutoInputStrategyType => autoInputStrategyType;

        public void InitAutoInputStrategy(AutoInputStrategyType autoInputStrategyType)
            => this.autoInputStrategyType = autoInputStrategyType;
    }
}
