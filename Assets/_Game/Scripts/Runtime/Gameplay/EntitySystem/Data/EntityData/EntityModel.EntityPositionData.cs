using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public partial class EntityModel : IEntityPositionData
    {
        protected Vector2 position;

        public Vector2 Position { get; set; }
    }
}
