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

            _autoInputStrategy = GetAutoInputStrategy(autoInputData.CurrentAutoInputStrategyType, controlData, statData, controlCastRange);
            if(_autoInputStrategy == null)
                return UniTask.FromResult(false);

            _controlData = controlData;
            _autoInputData.OnChangedAutoInputStrategy += OnChangedAutoInput;

            return UniTask.FromResult(true);
        }

        private void OnChangedAutoInput()
        {
            var controlCastRange = GetComponent<IEntityControlCastRangeProxy>();
            var newInputStrategy = GetAutoInputStrategy(_autoInputData.CurrentAutoInputStrategyType, _controlData, _statData, controlCastRange);
            if(newInputStrategy != null)
            {
                _autoInputStrategy.Dispose();
                _autoInputStrategy = newInputStrategy;
            }    
        }

        public IAutoInputStrategy GetAutoInputStrategy(AutoInputStrategyType autoInputStrategyType, IEntityControlData controlData, IEntityStatData statData, IEntityControlCastRangeProxy controlCastRange)
        {
            switch (autoInputStrategyType)
            {
                case AutoInputStrategyType.KeepDistanceToTarget:
                    return new KeepDistanceToTargetAutoInputStrategy(controlData, statData, controlCastRange);
                case AutoInputStrategyType.MoveTowardTarget:
                    return new MoveTowardTargetAutoInputStrategy(controlData, statData, controlCastRange);
                case AutoInputStrategyType.MoveByWay:
                    return new MoveByWayAutoInputStrategy(controlData, statData, controlCastRange);
                case AutoInputStrategyType.MoveRandomAroundTarget:
                    return new MoveRandomAroundTargetAutoInputStrategy(controlData, statData, controlCastRange);
                case AutoInputStrategyType.MoveOnPreDefinedPath:
                    return new MoveOnPreDefinedPathAutoInputStrategy(controlData, statData, controlCastRange);
                case AutoInputStrategyType.MoveOnPreDefinedPathFollowTarget:
                    return new MoveOnPreDefinedPathFollowTargetAutoInputStrategy(controlData, statData, controlCastRange);
                case AutoInputStrategyType.Idle:
                    return new IdleAutoInputStrategy(controlData, statData, controlCastRange);
                default:
                    return null;
            }
        }
    }
}