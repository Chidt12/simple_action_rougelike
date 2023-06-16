using Pathfinding;
using Runtime.Manager.Gameplay;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public class MoveRandomAroundTargetAutoInputStrategy : AutoInputStrategy
    {
        protected override float RefindTargetMinTime => 5;
        private const float DISTANCE_CHECK_AROUND = 7;

        public MoveRandomAroundTargetAutoInputStrategy(IEntityControlData controlData, IEntityStatData statData, IEntityControlCastRangeProxy entityControlCastRangeProxy) : base(controlData, statData, entityControlCastRangeProxy)
        {

        }

        protected override void FindNewPath()
        {
            var targetPosition = ControlData.Position;
            if(ControlData.Target != null)
            {
                var positionsAround = MapManager.Instance.GetWalkablePositionsAroundPosition(ControlData.Target.Position, DISTANCE_CHECK_AROUND, numberOfDirections: 32);
                targetPosition = positionsAround[Random.Range(0, positionsAround.Count)];
            }
            else
            {
                var positionsAround = MapManager.Instance.GetWalkablePositionsAroundPosition(ControlData.Position, DISTANCE_CHECK_AROUND, numberOfDirections: 32);
                targetPosition = positionsAround[Random.Range(0, positionsAround.Count)];
            }

            MapManager.Instance.FindPathWithRandomness(ControlData.Position,
                             targetPosition,
                             OnRunFindPathToTargetComplete);
        }

        private void OnRunFindPathToTargetComplete(Path path)
        {
            currentRefindTargetTime = 0.0f;
            if (!path.error && path.hasPath)
            {
                PathFoundCompleted(path.vectorPath);
            }
            else
            {
                findingNewPath = false;
            }
        }

        protected override void MoveOnPath() => Move();
    }
}
