using Cysharp.Threading.Tasks;
using Runtime.Core.Message;
using UnityEngine;
using ZBase.Foundation.PubSub;

namespace Runtime.Gameplay.EntitySystem
{
    public readonly struct SpawnedHeroMessage : IMessage { }

    [DisallowMultipleComponent]
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
            _subcription = SimpleMessenger.Subscribe<SpawnedHeroMessage>(OnHeroSpawned);
            return UniTask.FromResult(true);
        }

        private void OnHeroSpawned() => SearchForHero();

        private void SearchForHero()
        {
            _controlData.SetTarget((IEntityData)EntitiesManager.Instance.HeroData);
        }
    }
}
