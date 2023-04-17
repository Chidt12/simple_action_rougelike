using Pathfinding;
using Runtime.Manager.Gameplay;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public class MoveTowardTargetAutoInputStrategy : AutoInputStrategy
    {
        private static float s_stoppingDistance = 0.5f;
        private float _castRange;

        public MoveTowardTargetAutoInputStrategy(IEntityPositionData positionData, IEntityControlData controlData, IEntityStatData statData, float castRange, CustomRVOController rvoController) 
            : base(positionData, controlData, statData, castRange, rvoController)
        {
            _castRange = castRange;
        }

        protected override bool CanFindPath()
        {
            return base.CanFindPath() && ControlData.Target != null && !ControlData.Target.IsDead;
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
            if (!path.error && path.hasPath)
                PathFoundCompleted(path);
            else
                RunFindPathToAroundTarget();
        }

        private void RunFindPathToAroundTarget()
        {
            MapManager.Instance.FindNeighbourTargetPath(PositionData.Position,
                                                        ControlData.Target.Position,
                                                        OnRunFindPathToAroundTargetComplete);
        }

        private void OnRunFindPathToAroundTargetComplete(Path path)
        {
            if (!path.error && path.hasPath)
                PathFoundCompleted(path);
            else
                RunFindPathRandomly();
        }

        private void RunFindPathRandomly()
        {
            MapManager.Instance.FindNeighbourEmptyPath(PositionData.Position,
                                                       randomMoveSearchLength,
                                                       randomMoveSearchSpreadLength,
                                                       OnRunFindPathRandomlyComplete);
        }

        private void OnRunFindPathRandomlyComplete(Path path)
        {
            if (!path.error && path.hasPath)
            {
                PathFoundCompleted(path);
            }
            else
            {
                canFindNewPath = true;
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

            if (!IsObscured())
            {
                // If the chased target is now near the character by the skill cast range, then stop chasing and send a trigger skill usage.
                if (Vector2.SqrMagnitude(ControlData.Target.Position - PositionData.Position) <= _castRange * _castRange)
                {
                    // TRIGGER SKILL OR SOMETHING.
                }

                // If the target has moved far from the destination where the character was supposed to move to, then find another new path.
                if (Vector2.SqrMagnitude(ControlData.Target.Position - moveToPosition) >= refindTargetThresholdSqr && currentRefindTargetTime > RefindTargetMinTime)
                {
                    currentRefindTargetTime = 0.0f;
                    RefindNewPath();
                    return;
                }
            }

            // Make a move.
            Move();
        }
    }
}
