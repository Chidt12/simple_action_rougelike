using Runtime.Definition;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public interface IEntityData 
    { 
        public EntityType EntityType { get; }
        uint EntityUID { get; }
    }

    public interface IEntityPositionData : IEntityData
    {
        public Vector2 Position { get; set; }
    }
}
