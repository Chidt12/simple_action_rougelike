using Cysharp.Threading.Tasks;
using Runtime.Core.Message;
using Runtime.Definition;
using Runtime.Message;
using System.Collections.Generic;
using UnityEngine;
using ZBase.Foundation.PubSub;

namespace Runtime.Gameplay.EntitySystem
{
    [DisallowMultipleComponent]
    public class EntityGetAutoInputBehavior : EntityBehavior<IEntityControlData, IEntityStatData, IEntityAutoInputData>, IUpdateEntityBehavior , IDisposeEntityBehavior
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

        protected override UniTask<bool> BuildDataAsync(IEntityControlData controlData, IEntityStatData statData, IEntityAutoInputData autoInputData)
        {
            var controlCastRange = GetComponent<IEntityControlCastRangeProxy>();
            if(controlCastRange == null || statData == null || autoInputData == null)
                return UniTask.FromResult(false);

            _autoInputStrategy = GetAutoInputStrategy(autoInputData.AutoInputStrategyType, controlData, statData, controlCastRange);
            if(_autoInputStrategy == null)
                return UniTask.FromResult(false);

            _controlData = controlData;

            return UniTask.FromResult(true);
        }

        public IAutoInputStrategy GetAutoInputStrategy(AutoInputStrategyType autoInputStrategyType, IEntityControlData controlData, IEntityStatData statData, IEntityControlCastRangeProxy controlCastRange)
        {
            switch (autoInputStrategyType)
            {
                case AutoInputStrategyType.KeepDistanceToTarget:
                    return new KeepDistanceToTargetAutoInputStrategy(controlData, statData, controlCastRange);
                case AutoInputStrategyType.MoveTowardTarget:
                    return new MoveTowardTargetAutoInputStrategy(controlData, statData, controlCastRange);
                default:
                    return null;
            }
        }
    }
}