using Runtime.ConfigModel;
using Runtime.Gameplay.EntitySystem;
using ZBase.Foundation.PubSub;

namespace Runtime.Message
{
    public readonly struct EntityDiedMessage : IMessage
    {
        public readonly IEntityData EntityData;
        public readonly DeathDataIdentity DeathIdentity;

        public readonly bool IsHeroDied => EntityData.EntityType.IsHero();
        public readonly bool IsEnemyDied => EntityData.EntityType.IsEnemy();

        public EntityDiedMessage(IEntityData entityData, DeathDataIdentity deathDataIdentity)
        {
            EntityData = entityData;
            DeathIdentity = deathDataIdentity;
        }
    }
}
