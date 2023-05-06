using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    [DisallowMultipleComponent]
    public class EntityGetAutoInputBehavior : EntityBehavior<IEntityData, IEntityControlData, IEntityStatData>, IUpdateEntityBehavior , IDisposeEntityBehavior
    {
        private IAutoInputStrategy _autoInputStrategy;

        public void Dispose()
        {
            _autoInputStrategy.Dispose();
        }

        public void OnUpdate(float deltaTime)
        {
            _autoInputStrategy.Update();
        }

        protected override UniTask<bool> BuildDataAsync(IEntityData positionData, IEntityControlData controlData, IEntityStatData statData)
        {
            var controlCastRange = GetComponent<IEntityControlCastRangeProxy>();
            if(controlCastRange != null)
            {
                _autoInputStrategy = new KeepDistanceToTargetAutoInputStrategy(positionData, controlData, statData, controlCastRange);
            }
            else
            {
                return UniTask.FromResult(false);
            }
            
            return UniTask.FromResult(true);
        }


    }
}