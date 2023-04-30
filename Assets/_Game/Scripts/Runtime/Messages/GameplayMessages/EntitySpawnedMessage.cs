using Runtime.Gameplay.EntitySystem;
using UnityEngine;
using ZBase.Foundation.PubSub;

namespace Runtime.Message
{
    public readonly struct EntitySpawnedMessage : IMessage
    {
        #region Members

        public readonly IEntityPositionData EntityData;
        public readonly Transform EntityTransform;

        #endregion Members

        #region Struct Methods

        public EntitySpawnedMessage(IEntityPositionData entityData, Transform entityTransform)
        {
            EntityData = entityData;
            EntityTransform = entityTransform;
        }

        #endregion Struct Methods
    }
}
