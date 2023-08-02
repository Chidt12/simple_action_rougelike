using Pathfinding;
using Runtime.Manager.Gameplay;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public class MoveByWayAutoInputStrategy : AutoInputStrategy
    {
        protected override float RefindTargetMinTime => 4;

        public MoveByWayAutoInputStrategy(IEntityControlData controlData, IEntityStatData statData, IEntityControlCastRangeProxy entityControlCastRangeProxy) 
            : base(controlData, statData, entityControlCastRangeProxy)
        {

        }

        protected override void ReachedTheEndOfPath()
        {
            base.ReachedTheEndOfPath();
            // Force search path at the end.
            currentRefindTargetTime = RefindTargetMinTime;
        }

        protected override bool CanFindPath()
        {
            return base.CanFindPath()
                && ControlData.Target != null
                && !ControlData.Target.IsDead;
        }

        protected override void FindNewPath()
        {
            MapManager.Instance.FindStraightPath(ControlData.Position,
                                         ControlData.Target.Position,
                                         OnRunFindPathToTargetComplete);
        }

        private void OnRunFindPathToTargetComplete(Path path)
        {
            currentRefindTargetTime = 0.0f;
            if (!path.error && path.hasPath)
            {
                PathFoundCompleted(path.vectorPath, false);
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

            // If the target has moved far from the destination where the character was supposed to move to, then find another new path.
            if (Vector2.Distance(ControlData.Target.Position, moveToPosition) >= RefindTargetThreshold)
            {
                ResetToRefindNewPath(keepMovingOnPath: true);
                return;
            }
        }
    }
}