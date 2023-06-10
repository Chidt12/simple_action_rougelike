using Runtime.Definition;
using System;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public partial class EntityModel : IEntityData
    {
        protected int uid;
        protected int entityId;
        protected int level;
        protected EntityType entityType;

        public EntityType EntityType => entityType;
        public int EntityId => entityId;
        public int EntityUID => uid;
        public int Level => level;
        public bool IsDead => healthStat.CurrentValue <= 0;
        public bool CanDash => !(IsDashing || IsAttacking);
        public bool IsDamagable => !(IsDead || IsDashing || IsInvincible);
        public bool IsMovable => !(IsPlayingSkill || IsAttacking || IsDead || IsDashing || IsPausedControl || currentState.IsInHardCCStatus());
        public bool IsControllable => !(IsPlayingSkill || IsDead || IsPausedControl || currentState.IsInMovementLockedStatus());
        public bool IsDashing { get; set; }
        public bool IsPausedControl { get; set; }
        public bool IsInvincible { get; set; }
        public Vector2 Position { get; set; }
        public Action DeathEvent { get; set; }
        public Action<EntityReactionType> ReactionChangedEvent { get; set; }
        public Action<Vector2> ForceUpdatePosition { get; set; }
        public Transform EntityTransform { get; set; }

        public virtual void Init(EntityType entityType, int uid, int entityId, int level)
        {
            this.level = level;
            this.entityType = entityType;
            this.uid = uid;
            this.entityId = entityId;

            DeathEvent = () => { };
            DirectionChangedEvent = () => { };
            ReactionChangedEvent = _ => { };
            ForceUpdatePosition = _ => { };

            InitControl();
        }
    }
}