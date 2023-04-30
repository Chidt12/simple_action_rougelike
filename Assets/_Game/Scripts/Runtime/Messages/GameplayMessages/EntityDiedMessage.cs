using Runtime.Gameplay.EntitySystem;
using ZBase.Foundation.PubSub;

namespace Runtime.Message
{
    public readonly struct EntityDiedMessage : IMessage
    {
        #region Members

        public readonly IEntityPositionData EntityData;
        public readonly bool SpawnedEnemyAfterDeath;

        #endregion Members

        #region Properties

        public readonly bool IsHeroDied => EntityData.EntityType.IsHero();
        public readonly bool IsEnemyDied => EntityData.EntityType.IsEnemy();

        #endregion Properties

        #region Struct Methods

        public EntityDiedMessage(IEntityPositionData entityData, bool spawnedEnemyAfterDeath)
        {
            EntityData = entityData;
            SpawnedEnemyAfterDeath = spawnedEnemyAfterDeath;
        }

        #endregion Struct Methods
    }
}
