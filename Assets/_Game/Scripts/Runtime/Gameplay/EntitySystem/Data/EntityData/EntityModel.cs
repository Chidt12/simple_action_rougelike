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
        public Action<Vector2> ForceUpdatePosition { get; set; }

        public virtual void Init(EntityType entityType, int uid, int entityId)
        {
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