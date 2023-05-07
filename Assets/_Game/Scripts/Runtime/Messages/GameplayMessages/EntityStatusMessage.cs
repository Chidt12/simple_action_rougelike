using Runtime.Definition;
using Runtime.Gameplay.EntitySystem;
using ZBase.Foundation.PubSub;

namespace Runtime.Message
{
    public readonly struct SentStatusEffectMessage : IMessage
    {
        public readonly IEntityData Creator;
        public readonly IEntityStatusData Target;
        public readonly StatusType StatusType;
        public readonly int StatusDataId;
        public readonly StatusMetaData StatusMetaData;

        public SentStatusEffectMessage(IEntityData creator, IEntityStatusData target, StatusType statusType, int dataId, StatusMetaData statusMetaData = default)
        {
            Creator = creator;
            Target = target;
            StatusType = statusType;
            StatusDataId = dataId;
            StatusMetaData = statusMetaData;
        }
    }
}
