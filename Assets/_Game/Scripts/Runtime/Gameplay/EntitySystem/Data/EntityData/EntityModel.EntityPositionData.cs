using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public partial class EntityModel : IEntityPositionData
    {
        public Vector2 Position { get; set; }
    }
}
