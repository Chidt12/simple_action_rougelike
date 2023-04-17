using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    public partial class EntityModel : IEntityData
    {
        protected int uid;
        protected EntityType entityType;

        public EntityType EntityType => entityType;

        public int EntityUID => uid;

        public bool IsDead => healthStat.CurrentValue <= 0;

        public void Init(EntityType entityType, int uid)
        {
            this.entityType = entityType;
            this.uid = uid;
            InitStats();
            InitControl();
        }
    }
}