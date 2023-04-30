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

        public virtual void Init(EntityType entityType, int uid)
        {
            this.entityType = entityType;
            this.uid = uid;
            InitStats();
            InitControl();
        }
    }

    public static class EntityModelExtensions
    {
        #region Class Methods

        public static bool IsHero(this EntityType entityType) => entityType == EntityType.Hero;
        public static bool IsBoss(this EntityType entityType) => entityType == EntityType.Boss;
        public static bool IsEnemy(this EntityType entityType) => entityType == EntityType.Enemy || entityType == EntityType.Boss;
        public static bool IsCharacter(this EntityType entityType) => entityType == EntityType.Hero || entityType == EntityType.Enemy || entityType == EntityType.Boss;
        public static bool IsObject(this EntityType entityType) => entityType == EntityType.Object;
        public static bool IsTrap(this EntityType entityType) => entityType == EntityType.Trap;

        public static bool CanCauseDamage(this EntityType entityType, EntityType targetEntityType)
        {
            if (entityType == targetEntityType)
                return false;
            else if (entityType.IsEnemy() && targetEntityType.IsEnemy())
                return false;
            else if (targetEntityType == EntityType.Object)
                return entityType == EntityType.Hero;
            return true;
        }

        #endregion Class Methods
    }
}