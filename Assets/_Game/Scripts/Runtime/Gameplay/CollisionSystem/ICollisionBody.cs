using System;
using UnityEngine;
using Runtime.Definition;

namespace Runtime.Gameplay.CollisionDetection
{
    public enum CollisionSearchTargetType : int
    {
        None = -1,
        All = 0,
        EnemyAndObject = 1,
        Hero = 2,
        Projectile = 3,
        Enemy = 4,
        Object = 5
    }

    public enum CollisionBodyType
    {
        Default,
        TargetDetect,
        Hero,
        Zombie,
        Projectile,
        Object,
        Trap,
        DamageArea
    }

    public static class CollisionBodyExtensions
    {
        #region Class Methods

        public static CollisionSearchTargetType GetCollisionBodySearchType(this EntityType entityType)
        {
            if (entityType == EntityType.Boss || entityType == EntityType.Enemy)
                return CollisionSearchTargetType.Hero;
            else if (entityType == EntityType.Hero)
                return CollisionSearchTargetType.EnemyAndObject;
            return CollisionSearchTargetType.All;
        }

        public static CollisionBodyType GetCollisionBodyType(this EntityType entityType)
        {
            if (entityType == EntityType.Boss || entityType == EntityType.Enemy)
                return CollisionBodyType.Zombie;
            else if (entityType == EntityType.Hero)
                return CollisionBodyType.Hero;
            else if (entityType == EntityType.Object)
                return CollisionBodyType.Object;
            else if (entityType == EntityType.Trap)
                return CollisionBodyType.Trap;
            return CollisionBodyType.Default;
        }

        public static ICollisionShape CreateCollisionShape(this ICollisionBody collisionBody, Collider2D collider)
        {
            if (collisionBody.Collider != null)
            {
                if (collider is BoxCollider2D)
                    return new RectangleCollisionShape(collisionBody, collider.bounds.extents.x * 2, collider.bounds.extents.y * 2);
                else if (collider is CircleCollider2D)
                    return new CircleCollisionShape(collisionBody, collider.bounds.extents.x);
                else
                    return new ColliderCollisionShape(collisionBody);
            }
            return null;
        }

        public static ICollisionShape CreateCollisionShape(this ICollisionBody collisionBody, float radius)
            => new CircleCollisionShape(collisionBody, radius);

        #endregion Class Methods
    }

    public interface ICollisionBody
    {
        #region Properties

        int RefId { get; set; }
        ICollisionShape CollisionShape { get; }
        CollisionSearchTargetType CollisionSearchTargetType { get; }
        CollisionBodyType CollisionBodyType { get; }
        Collider2D Collider { get; }
        Vector2 CollisionSystemPosition { get; }

        #endregion Properties

        #region Interface Methods

        void OnCollision(CollisionResult result, ICollisionBody other);

        #endregion Interface Methods
    }
}