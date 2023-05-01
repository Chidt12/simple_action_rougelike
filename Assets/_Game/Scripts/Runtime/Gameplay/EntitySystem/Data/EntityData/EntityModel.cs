using Runtime.Definition;
using System;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public partial class EntityModel : IEntityData
    {
        protected int uid;
        protected int entityId;
        protected EntityType entityType;

        public EntityType EntityType => entityType;
        public int EntityId => entityId;
        public int EntityUID => uid;
        public bool IsDead => healthStat.CurrentValue <= 0;
        public bool IsDamagable => true;
        public Vector2 Position { get; set; }

        public Action DeathEvent { get; set; }
        public Action<EntityReactionType> ReactionChangedEvent { get; set; }

        public virtual void Init(EntityType entityType, int uid, int entityId)
        {
            this.entityType = entityType;
            this.uid = uid;
            this.entityId = entityId;

            DeathEvent = () => { };
            DirectionChangedEvent = () => { };
            ReactionChangedEvent = _ => { };

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