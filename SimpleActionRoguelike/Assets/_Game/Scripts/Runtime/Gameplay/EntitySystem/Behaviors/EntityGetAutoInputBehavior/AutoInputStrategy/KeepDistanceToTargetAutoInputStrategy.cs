using Pathfinding;
using Runtime.Manager.Gameplay;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public class KeepDistanceToTargetAutoInputStrategy : AutoInputStrategy
    {
        private enum MoveState
        {
            MoveTowardsHero,
            MoveAwayFromHero,
        }

        private static readonly float s_stayBeforeAwayTargetBonusRange = 1.0f;
        private static readonly int s_awayMoveSearchMinSlotsCount = 1;
        private float StayBeforeAwayTargetDistance => ControlCastRangeProxy.CastRange - s_stayBeforeAwayTargetBonusRange;
        private bool _isMoveAwayFromTarget;
        private MoveState _moveState = MoveState.MoveTowardsHero;

        public KeepDistanceToTargetAutoInputStrategy(IEntityControlData controlData, IEntityStatData statData, IEntityControlCastRangeProxy controlCastRangeProxy)
            : base(controlData, statData, controlCastRangeProxy)
        {
            _isMoveAwayFromTarget = false;
        }

        protected override bool CanFindPath()
        {
            return base.CanFindPath()
                && ControlData.Target != null
                && !ControlData.Target.IsDead;
        }

        protected override void MoveOnPath()
        {
            if (ControlData.Target == null || ControlData.Target.IsDead)
            {
                LockMovement();
                return;
            }

            Move();

            if (_moveState == MoveState.MoveTowardsHero)
            {
                // If the chased hero target is far away the character by a specific distance, then make the character move away from the hero target.
                if (Vector2.Distance(ControlData.Target.Position, ControlData.Position) <= StayBeforeAwayTargetDistance )
                {
                    _isMoveAwayFromTarget = true;
                    ResetToRefindNewPath();
                    return;
                }

                // If the chased hero target has moved far from the destination where the character was supposed to move to, then find another new path.
                if (Vector2.Distance(ControlData.Target.Position, moveToPosition) >= RefindTargetThreshold)
                {
                    ResetToRefindNewPath();
                    return;
                }
            }
            else if (_moveState == MoveState.MoveAwayFromHero)
            {
                // If the chased hero target is now far from the character by a specific distance, then find a new path to chase the hero again.
                if (Vector2.Distance(ControlData.Target.Position, ControlData.Position) > StopChasingTargetDistance)
                {
                    _isMoveAwayFromTarget = false;
                    ResetToRefindNewPath();
                    return;
                }
            }
        }

        protected override void FindNewPath()
        {
            if (_isMoveAwayFromTarget)
            {
                MapManager.Instance.FindMoveAwayTargetReal(ControlData.Position,
                                                       ControlData.Target.Position,
                                                       ControlCastRangeProxy.CastRange,
                                                       s_awayMoveSearchMinSlotsCount,
                                                       OnRunFindPathAwayTargetComplete);
            }
            else
            {
                MapManager.Instance.FindStraightPath(ControlData.Position,
                                         ControlData.Target.Position,
                                         OnRunFindPathTowardsTargetComplete);
            }
        }

        private void OnRunFindPathAwayTargetComplete(Path path)
        {
            if (!path.error && path.hasPath)
            {
                _moveState = MoveState.MoveAwayFromHero;
                PathFoundCompleted(path.vectorPath);
            }
            else
            {
                ResetToRefindNewPath();
            }
        }

        private void OnRunFindPathTowardsTargetComplete(Path path)
        {
            if (!path.error && path.hasPath)
            {
                _moveState = MoveState.MoveTowardsHero;
                PathFoundCompleted(path.vectorPath);
            }
            else
            {
                ResetToRefindNewPath();
            }
        }
    }

}