using Cysharp.Threading.Tasks;
using Runtime.ConfigModel;
using Runtime.Manager.Data;
using System.Threading;

namespace Runtime.Gameplay.EntitySystem
{
    public abstract class DeathStrategy<T> : IDeathStrategy where T : DeathDataConfigItem
    {
        public async UniTask Execute(IEntityData entityData, DeathDataIdentity deathDataIdentity, CancellationToken cancellationToken)
        {
            var dataConfigItem = await ConfigDataManager.Instance.LoadDeathDataConfigItem(deathDataIdentity);
            await Execute(entityData, dataConfigItem as T, cancellationToken);
        }

        protected abstract UniTask Execute(IEntityData entityData, T deathDataConfig, CancellationToken cancellationToken);
    }

    public interface IDeathStrategy
    {
        public UniTask Execute(IEntityData entityData, DeathDataIdentity deathDataIdentity, CancellationToken cancellationToken);
    }
}