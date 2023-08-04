using Cysharp.Threading.Tasks;
using Runtime.Definition;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    [DisallowMultipleComponent]
    public class EntityGetAutoInputBehavior : EntityBehavior<IEntityControlData, IEntityStatData, IEntityAutoInputData>, IUpdateEntityBehavior , IDisposeEntityBehavior
    {
        private IAutoInputStrategy _autoInputStrategy;
        private IEntityControlData _controlData;
        private IEntityAutoInputData _autoInputData;
        private IEntityStatData _statData;

        public void Dispose()
        {
            _autoInputStrategy.Dispose();
        }

        public void OnUpdate(float deltaTime)
        {
            if(_controlData.IsControllable)
                _autoInputStrategy.Update();
        }

        protected override UniTask<bool> BuildDataAsync(IEntityControlData controlData, IEntityStatData statData, IEntityAutoInputData autoInputData)
        {
            var controlCastRange = GetComponent<IEntityControlCastRangeProxy>();
            if(controlCastRange == null || statData == null || autoInputData == null)
                return UniTask.FromResult(false);

            _controlData = controlData;
            _statData = statData;
            _autoInputData = autoInputData;

            _autoInputStrategy = AutoInputStrategyFactory.GetAutoInputStrategy(autoInputData.CurrentAutoInputStrategyType, controlData, statData, controlCastRange);
            if(_autoInputStrategy == null)
                return UniTask.FromResult(false);

            _controlData = controlData;
            _autoInputData.OnChangedAutoInputStrategy += OnChangedAutoInput;

            return UniTask.FromResult(true);
        }

        private void OnChangedAutoInput()
        {
            var controlCastRange = GetComponent<IEntityControlCastRangeProxy>();
            var newInputStrategy = AutoInputStrategyFactory.GetAutoInputStrategy(_autoInputData.CurrentAutoInputStrategyType, _controlData, _statData, controlCastRange);
            if(newInputStrategy != null)
            {
                _autoInputStrategy.Dispose();
                _autoInputStrategy = newInputStrategy;
            }    
        }

        
    }
}