using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    [DisallowMultipleComponent]
    public class EntityGetAutoInputBehavior : EntityBehavior<IEntityControlData, IEntityStatData>, IUpdateEntityBehavior , IDisposeEntityBehavior
    {
        private IAutoInputStrategy _autoInputStrategy;
        private IEntityControlData _controlData;

        public void Dispose()
        {
            _autoInputStrategy.Dispose();
        }

        public void OnUpdate(float deltaTime)
        {
            if(_controlData.IsControllable)
                _autoInputStrategy.Update();
        }

        protected override UniTask<bool> BuildDataAsync(IEntityControlData controlData, IEntityStatData statData)
        {
            var controlCastRange = GetComponent<IEntityControlCastRangeProxy>();
            if(controlCastRange != null)
            {
                _autoInputStrategy = new KeepDistanceToTargetAutoInputStrategy(controlData, statData, controlCastRange);
            }
            else
            {
                return UniTask.FromResult(false);
            }

            _controlData = controlData;
            return UniTask.FromResult(true);
        }


    }
}