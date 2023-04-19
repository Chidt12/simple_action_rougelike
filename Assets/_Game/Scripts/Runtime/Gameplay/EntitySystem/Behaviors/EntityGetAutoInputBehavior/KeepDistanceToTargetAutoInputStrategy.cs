using Pathfinding;
using Runtime.Manager.Gameplay;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public class KeepDistanceToTargetAutoInputStrategy : AutoInputStrategy
    {
        private enum MoveState
        {
            MoveTowardsHero,
            MoveAwayFromHero,
            MoveRandomly,
        }

        private static readonly float s_stayBeforeAwayTargetBonusRange = 1.0f;
        private static readonly int s_awayMoveSearchMinSlotsCount = 3;
        private static readonly int s_awayMoveSearchMaxSlotsCount = 6;
        private static readonly float s_awayMoveAimStrength = 0.5f;
        private float _stayBeforeAwayTargetDistanceSqr;
        private bool _isMoveAwayFromTarget;
        private int _awayMoveSearchLength;
        private int _awayMoveSearchSpreadLength;
        private MoveState _moveState = MoveState.MoveTowardsHero;

        public KeepDistanceToTargetAutoInputStrategy(IEntityPositionData positionData, IEntityControlData controlData, IEntityStatData statData, float castRange)
            : base(positionData, controlData, statData, castRange)
        {
            _isMoveAwayFromTarget = false;
            _stayBeforeAwayTargetDistanceSqr = (castRange - s_stayBeforeAwayTargetBonusRange) * (castRange - s_stayBeforeAwayTargetBonusRange);
            _awayMoveSearchLength = Mathf.CeilToInt(s_awayMoveSearchMinSlotsCount * MapManager.Instance.SlotSize) * PATH_FINDING_COST_MULTIPLIER;
            _awayMoveSearchSpreadLength = Mathf.CeilToInt(s_awayMoveSearchMaxSlotsCount * MapManager.Instance.SlotSize) * PATH_FINDING_COST_MULTIPLIER;
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

            // Make a move.
            Move();

            if (_moveState == MoveState.MoveTowardsHero)
            {
                if (!IsObscured())
                {
                    // If the chased hero target is far away the character by a specific distance, then make the character move away from the hero target.
                    if (Vector2.SqrMagnitude(ControlData.Target.Position - PositionData.Position) <= _stayBeforeAwayTargetDistanceSqr)
                    {
                        _isMoveAwayFromTarget = true;
                        ResetToRefindNewPath();
                        // use skill
                        return;
                    }

                    // If the chased hero target is now near the character by the skill cast range, then stop chasing, send a trigger skill usage.
                    if (Vector2.SqrMagnitude(PositionData.Position - ControlData.Target.Position) <= (stopChasingTargetDistanceSqr))
                    {
                        LockMovement();
                        // Use skill
                        return;
                    }

                    // If the chased hero target has moved far from the destination where the character was supposed to move to, then find another new path.
                    if (Vector2.SqrMagnitude(ControlData.Target.Position - moveToPosition) >= refindTargetThresholdSqr)
                    {
                        ResetToRefindNewPath();
                        return;
                    }
                }
            }
            else if (_moveState == MoveState.MoveAwayFromHero)
            {
                // If the chased hero target is now far from the character by a specific distance, then find a new path to chase the hero again.
                if (Vector2.SqrMagnitude(ControlData.Target.Position - PositionData.Position) > stopChasingTargetDistanceSqr)
                {
                    _isMoveAwayFromTarget = false;
                    ResetToRefindNewPath();
                    return;
                }
            }
        }

        protected override void FindNewPath()
            => RunFindPath();

        private void RunFindPath()
        {
            if (_isMoveAwayFromTarget)
                RunFindPathAwayTarget();
            else
                RunFindPathTowardsTarget();
        }

        private void RunFindPathAwayTarget()
        {
            MapManager.Instance.FindMoveAwayTargetPath(PositionData.Position,
                                                       ControlData.Target.Position,
                                                       _awayMoveSearchLength,
                                                       _awayMoveSearchSpreadLength,
                                                       s_awayMoveAimStrength,
                                                       OnRunFindPathAwayTargetComplete);
        }

        private void OnRunFindPathAwayTargetComplete(Path path)
        {
            if (!path.error && path.hasPath)
            {
                _moveState = MoveState.MoveAwayFromHero;
                PathFoundCompleted(path);
            }
            else RunFindPathRandomly();
        }

        private void RunFindPathTowardsTarget()
        {
            MapManager.Instance.FindPath(PositionData.Position,
                                         ControlData.Target.Position,
                                         OnRunFindPathTowardsTargetComplete);
        }

        private void OnRunFindPathTowardsTargetComplete(Path path)
        {
            if (!path.error && path.hasPath)
            {
                _moveState = MoveState.MoveTowardsHero;
                PathFoundCompleted(path);
            }
            else RunFindPathRandomly();
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
                _moveState = MoveState.MoveRandomly;
                PathFoundCompleted(path);
            }
            else
            {
                canFindNewPath = true;
                hasFoundAPath = false;
            }
        }
    }

}