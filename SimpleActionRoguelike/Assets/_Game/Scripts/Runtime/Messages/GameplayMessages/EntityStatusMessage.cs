using Runtime.ConfigModel;
using Runtime.Gameplay.EntitySystem;
using ZBase.Foundation.PubSub;

namespace Runtime.Message
{
    public readonly struct SentStatusEffectMessage : IMessage
    {
        public readonly IEntityData Creator;
        public readonly IEntityStatusData Target;
        public readonly StatusIdentity StatusIdentity;
        public readonly StatusMetaData StatusMetaData;

        public SentStatusEffectMessage(IEntityData creator, IEntityStatusData target, StatusIdentity statusIdentity, StatusMetaData statusMetaData = default)
        {
            Creator = creator;
            Target = target;
            StatusIdentity = statusIdentity;
            StatusMetaData = statusMetaData;
        }
    }
}
