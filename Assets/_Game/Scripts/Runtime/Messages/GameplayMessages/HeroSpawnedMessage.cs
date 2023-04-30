using Runtime.Gameplay.EntitySystem;
using UnityEngine;
using ZBase.Foundation.PubSub;

namespace Runtime.Message
{
    public readonly struct HeroSpawnedMessage : IMessage
    {
        #region Members

        public readonly IEntityPositionData EntityData;
        public readonly Transform HeroTransform;

        #endregion Members

        #region Struct Methods

        public HeroSpawnedMessage(IEntityPositionData entityData, Transform heroTransform)
        {
            EntityData = entityData;
            HeroTransform = heroTransform;
        }

        #endregion Struct Methods
    }
}