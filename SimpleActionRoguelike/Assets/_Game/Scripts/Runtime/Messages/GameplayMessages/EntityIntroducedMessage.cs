using Runtime.Definition;
using ZBase.Foundation.PubSub;

namespace Runtime.Message
{
    public readonly struct EntityIntroducedMessage : IMessage
    {
        public readonly int EntityId;
        public readonly EntityType EntityType;
        public readonly float IntroduceTime;

        public EntityIntroducedMessage(int entityId, EntityType entityType, float introduceTime)
        {
            EntityId = entityId;
            EntityType = entityType;
            IntroduceTime = introduceTime;
        }
    }
}