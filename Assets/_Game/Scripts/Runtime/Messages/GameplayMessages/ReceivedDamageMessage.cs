using Runtime.Gameplay.EntitySystem;
using ZBase.Foundation.PubSub;

namespace Runtime.Message
{
    public readonly struct ReceivedDamageMessage : IMessage
    {
        public readonly EffectSource DamageSource;
        public readonly EffectProperty DamageProperty;
        public readonly float DamageTaken;
    }
}