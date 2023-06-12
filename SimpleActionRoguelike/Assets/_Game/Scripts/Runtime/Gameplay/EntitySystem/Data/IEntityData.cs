using Runtime.Core.Message;
using Runtime.Definition;
using System;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public interface IEntityData 
    { 
        public EntityType EntityType { get; }
        public int EntityId { get; }
        public int Level { get; }
        public bool IsDead { get; }
        public bool IsDamagable { get; }
        public bool IsControllable { get; }
        public bool IsMovable { get; }
        public bool IsDashing { get; set; }
        public bool IsPausedControl { get; set; }
        public bool IsPausedMove { get; set; }
        public bool IsInvincible { get; set; }
        int EntityUID { get; }
        public Vector2 Position { get; set; }
        public Transform EntityTransform { get; set; }
        Action DeathEvent { get; set; }
        Action<EntityReactionType> ReactionChangedEvent { get; set; }
        Action<Vector2> ForceUpdatePosition { get; set; }
        public bool CanDash { get; }
    }

    public static class EntityModelExtensions
    {
        #region Class Methods

        public static bool IsHero(this EntityType entityType) => entityType == EntityType.Hero;
        public static bool IsBoss(this EntityType entityType) => entityType == EntityType.Boss;
        public static bool IsEnemy(this EntityType entityType) => entityType == EntityType.Enemy || entityType == EntityType.Boss;
        public static bool IsCharacter(this EntityType entityType) => entityType == EntityType.Hero || entityType == EntityType.Enemy || entityType == EntityType.Boss;
        public static bool IsObject(this EntityType entityType) => entityType == EntityType.Asset;
        public static bool IsTrap(this EntityType entityType) => entityType == EntityType.Trap;

        public static bool CanCauseDamage(this EntityType entityType, EntityType targetEntityType)
        {
            if (entityType == targetEntityType)
                return false;
            else if (entityType.IsEnemy() && targetEntityType.IsEnemy())
                return false;
            else if (targetEntityType == EntityType.Asset)
                return entityType == EntityType.Hero;
            return true;
        }

        #endregion Class Methods
    }
}
