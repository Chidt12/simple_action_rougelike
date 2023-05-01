using Runtime.Definition;
using System;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public interface IEntityData 
    { 
        public EntityType EntityType { get; }
        public int EntityId { get; }
        public bool IsDead { get; }
        public bool IsDamagable { get; }
        int EntityUID { get; }
        public Vector2 Position { get; set; }

        Action DeathEvent { get; set; }
        Action<EntityReactionType> ReactionChangedEvent { get; set; }
    }
}
