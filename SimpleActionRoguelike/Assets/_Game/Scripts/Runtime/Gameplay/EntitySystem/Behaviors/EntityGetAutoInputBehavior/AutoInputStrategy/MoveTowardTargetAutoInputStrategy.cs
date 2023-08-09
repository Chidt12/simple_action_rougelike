using Pathfinding;
using Runtime.Manager.Gameplay;
using TMPro;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public class MoveTowardTargetAutoInputStrategy : AutoInputStrategy
    {
        private static float s_stoppingDistance = 0.5f;
        protected override float RefindTargetMinTime => 1;

        public MoveTowardTargetAutoInputStrategy(IEntityControlData controlData, IEntityStatData statData, IEntityControlCastRangeProxy controlCastRangeProxy) 
            : base(controlData, statData, controlCastRangeProxy)
        {
        }

        protected override bool CanFindPath()
        {
            return base.CanFindPath() 
                && ControlData.Target != null 
                && !ControlData.Target.IsDead
                && Vector2.Distance(ControlData.Position, ControlData.Target.Position) > s_stoppingDistance;
        }

        protected override void FindNewPath()
        {
            // Avoid get the same path.
            var positionsAround = MapManager.Instance.GetWalkablePositionsAroundPosition(ControlData.Target.Position, 1.5f, numberOfDirections: 32);
            var targetPosition = positionsAround[Random.Range(0, positionsAround.Count)];

            MapManager.Instance.FindPathWithRandomness(ControlData.Position,
                                         targetPosition,
                                         OnRunFindPathToTargetComplete);
        }

        private void OnRunFindPathToTargetComplete(Path path)
        {
            if (!findingNewPath)
                return;

            currentRefindTargetTime = 0.0f;
            if (!path.error && path.hasPath)
            {
                PathFoundCompleted(path.vectorPath);
            }
            else
            {
                findingNewPath = false;
                hasFoundAPath = false;
            }
        }

        protected override void MoveOnPath()
        {
            // Stand still if the target is dead.
            if (ControlData.Target != null && ControlData.Target.IsDead)
            {
                LockMovement();
                return;
            }

            // Make a move.
            Move();

            // If the chased target is now near the character by the skill cast range, then stop chasing and send a trigger skill usage.
            var distanceToTarget = Vector2.Distance(ControlData.Target.Position, ControlData.Position);
            if (distanceToTarget <= ControlCastRangeProxy.CastRange)
            {
                return;
            }

            // If the target has moved far from the destination where the character was supposed to move to, then find another new path.
            if (Vector2.Distance(ControlData.Target.Position, moveToPosition) >= RefindTargetThreshold)
            {
                ResetToRefindNewPath();
                LockMovement();
                return;
            }
        }
    }
}
