using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    public static class AutoInputStrategyFactory
    {
        public static IAutoInputStrategy GetAutoInputStrategy(AutoInputStrategyType autoInputStrategyType, IEntityControlData controlData, IEntityStatData statData, IEntityControlCastRangeProxy controlCastRange)
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