using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public class EntityGetAutoInputBehavior : EntityBehavior<IEntityPositionData, IEntityControlData, IEntityStatData>, IUpdateEntityBehavior , IDisposeEntityBehavior
    {
        [SerializeField]
        private CustomRVOController _rvoController;
        private IAutoInputStrategy _autoInputStrategy;

        public void Dispose()
        {
            _autoInputStrategy.Dispose();
        }

        public void OnUpdate(float deltaTime)
        {
            _autoInputStrategy.Update();
        }

        protected override UniTask<bool> BuildDataAsync(IEntityPositionData positionData, IEntityControlData controlData, IEntityStatData statData)
        {
            _autoInputStrategy = new MoveTowardTargetAutoInputStrategy(positionData, controlData, statData, 5f, _rvoController);
            return UniTask.FromResult(true);
        }


    }
}