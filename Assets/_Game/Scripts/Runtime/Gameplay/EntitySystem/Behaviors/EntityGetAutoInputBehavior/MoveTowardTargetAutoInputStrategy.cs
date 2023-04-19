using Pathfinding;
using Runtime.Manager.Gameplay;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public class MoveTowardTargetAutoInputStrategy : AutoInputStrategy
    {
        private static float s_stoppingDistance = 0.5f;
        private float _castRange;

        public MoveTowardTargetAutoInputStrategy(IEntityPositionData positionData, IEntityControlData controlData, IEntityStatData statData, float castRange) 
            : base(positionData, controlData, statData, castRange)
        {
            _castRange = castRange;
        }

        protected override bool CanFindPath()
        {
            return base.CanFindPath() 
                && ControlData.Target != null 
                && !ControlData.Target.IsDead
                && Vector2.Distance(PositionData.Position, ControlData.Target.Position) > s_stoppingDistance;
        }

        protected override void FindNewPath() => RunFindPathToTarget();

        private void RunFindPathToTarget()
        {
            MapManager.Instance.FindPath(PositionData.Position,
                                         ControlData.Target.Position,
                                         OnRunFindPathToTargetComplete);
        }

        private void OnRunFindPathToTargetComplete(Path path)
        {
            currentRefindTargetTime = 0.0f;
            if (!path.error && path.hasPath)
            {
                PathFoundCompleted(path);
            }
            else
            {
                canFindNewPath = true;
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

            if (!IsObscured())
            {
                // If the chased target is now near the character by the skill cast range, then stop chasing and send a trigger skill usage.
                var distanceToTarget = Vector2.Distance(ControlData.Target.Position, PositionData.Position);
                if (distanceToTarget <=  _castRange)
                {
                    // TRIGGER SKILL OR SOMETHING.
                    return;
                }

                // If the target has moved far from the destination where the character was supposed to move to, then find another new path.
                if (Vector2.SqrMagnitude(ControlData.Target.Position - moveToPosition) >= refindTargetThresholdSqr && currentRefindTargetTime > RefindTargetMinTime)
                {
                    ResetToRefindNewPath();
                    return;
                }
            }
        }
    }
}
