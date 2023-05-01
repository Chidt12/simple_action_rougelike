using Cysharp.Threading.Tasks;
using Runtime.Core.Pool;
using System.Threading;

namespace Runtime.Gameplay.EntitySystem
{
    public class EntityPresentDiedBehavior : EntityBehavior
    {
        public async override UniTask<bool> BuildAsync(IEntityData data, CancellationToken cancellationToken)
        {
            await base.BuildAsync(data, cancellationToken);
            data.DeathEvent += OnDeath;
            return true;
        }

        private void OnDeath()
        {
            PoolManager.Instance.Return(gameObject);
        }
    }
}
