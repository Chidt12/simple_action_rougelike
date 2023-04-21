using Cysharp.Threading.Tasks;
using Runtime.Core.Message;
using ZBase.Foundation.PubSub;

namespace Runtime.Gameplay.EntitySystem
{
    public readonly struct SpawnedHeroMessage : IMessage { }

    public class EntityDetectHeroLikeTargetBehavior : EntityBehavior<IEntityControlData>, IDisposeEntityBehavior
    {
        #region Members

        private IEntityControlData _controlData;
        private ISubscription _subcription;

        public void Dispose()
        {
            _subcription.Dispose();
        }

        #endregion Members

        protected override UniTask<bool> BuildDataAsync(IEntityControlData data)
        {
            _controlData = data;
            SearchForHero();
            var registry = SimpleMessenger.Subscribe<SpawnedHeroMessage>(OnHeroSpawned);
            return UniTask.FromResult(true);
        }

        private void OnHeroSpawned() => SearchForHero();

        private void SearchForHero()
        {
            _controlData.SetTarget(EntitiesManager.Instance.HeroModel);
        }
    }
}
