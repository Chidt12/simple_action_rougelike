using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    public partial class EntityModel : IEntityData
    {
        protected uint uid;

        public EntityType EntityType => EntityType.Hero;

        public uint EntityUID => uid;

        public bool IsDead => healthStat.CurrentValue <= 0;

        public void Init()
        {
            InitStats();
            InitControl();
        }
    }
}