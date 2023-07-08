using Cysharp.Threading.Tasks;
using Runtime.ConfigModel;
using System.Threading;

namespace Runtime.Gameplay.EntitySystem
{
    public class SpawnEntitiesDeathStrategy : DeathStrategy<SpawnEntitiesDeathDataConfigItem>
    {
        private static readonly float s_entitySpawnFromCharacterOffset = 1.5f;

        protected override UniTask Execute(IEntityData entityData, SpawnEntitiesDeathDataConfigItem deathDataConfig, CancellationToken cancellationToken)
        {
            return EntitiesManager.Instance.CreateEntitiesAsync(entityData.Position, s_entitySpawnFromCharacterOffset, false, false,
                                                         cancellationToken, deathDataConfig.spawnEntityInfo);
        }
    }
}