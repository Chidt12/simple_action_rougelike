using Runtime.ConfigModel;
using Runtime.Gameplay.EntitySystem;
using ZBase.Foundation.PubSub;

namespace Runtime.Message
{
    public readonly struct EntityDiedMessage : IMessage
    {
        #region Members

        public readonly IEntityData EntityData;
        public readonly bool SpawnedEnemyAfterDeath;
        public readonly DeathDataIdentity DeathIdentity;

        #endregion Members

        #region Properties

        public readonly bool IsHeroDied => EntityData.EntityType.IsHero();
        public readonly bool IsEnemyDied => EntityData.EntityType.IsEnemy();

        #endregion Properties

        #region Struct Methods

        public EntityDiedMessage(IEntityData entityData, DeathDataIdentity deathDataIdentity, bool spawnedEnemyAfterDeath)
        {
            EntityData = entityData;
            SpawnedEnemyAfterDeath = spawnedEnemyAfterDeath;
            DeathIdentity = deathDataIdentity;
        }

        #endregion Struct Methods
    }
}
