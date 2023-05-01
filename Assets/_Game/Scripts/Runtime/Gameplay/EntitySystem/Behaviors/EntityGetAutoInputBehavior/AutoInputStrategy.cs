using Pathfinding;
using Pathfinding.RVO;
using Pathfinding.Util;
using Runtime.Definition;
using Runtime.Manager.Gameplay;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public abstract class AutoInputStrategy : IAutoInputStrategy
    {
        #region Members

        protected const int PATH_FINDING_COST_MULTIPLIER = 1000;
        protected const float REACH_END_DISTANCE = 0.2f;

        protected static readonly float RefindTargetBonusRange = 2.0f;
        protected static readonly float RefindTargetMinTime = 1f;

        protected static readonly int RandomMoveSearchSlotsCount = 3;
        protected static readonly int RandomMoveSearchSpreadSlotsCount = 2;

        protected static readonly float CheckObscurationFragmentDistance = 1 / 5.0f;
        protected static readonly float CheckSideObscurationCount = 0;

        protected bool reachedEndOfPath;
        protected bool canFindNewPath;
        protected bool hasFoundAPath;
        protected float moveSpeed;
        protected float stopChasingTargetDistanceSqr;
        protected float stopChasingTargetDistance;
        protected float refindTargetThresholdSqr;
        protected float currentRefindTargetTime;

        protected int randomMoveSearchLength;
        protected int randomMoveSearchSpreadLength;

        protected List<Vector3> pathPositions;
        protected Vector2 moveToPosition;
        protected int currentPathPositionIndex;

        // Custom parameters
        protected IEntityData target;

        #endregion Members

        #region Properties
        protected IEntityControlData ControlData { get; private set; }
        protected IEntityData PositionData { get; private set; }

        #endregion Properties

        #region Class Methods

        public AutoInputStrategy(IEntityData positionData, IEntityControlData controlData, IEntityStatData statData, float castRange)
        {
            ControlData = controlData;
            PositionData = positionData;
            canFindNewPath = true;
            hasFoundAPath = false;
            stopChasingTargetDistanceSqr = castRange * castRange;
            stopChasingTargetDistance = castRange;
            refindTargetThresholdSqr = (castRange + RefindTargetBonusRange) * (castRange + RefindTargetBonusRange);

            randomMoveSearchLength = Mathf.CeilToInt(RandomMoveSearchSlotsCount * MapManager.Instance.SlotSize) * PATH_FINDING_COST_MULTIPLIER;
            randomMoveSearchSpreadLength = Mathf.CeilToInt(RandomMoveSearchSpreadSlotsCount * MapManager.Instance.SlotSize) * PATH_FINDING_COST_MULTIPLIER;

            if (statData.TryGetStat(StatType.MoveSpeed, out var statSpeed))
            {
                moveSpeed = statSpeed.TotalValue;
                statSpeed.OnValueChanged += OnStatChanged;
            }
#if DEBUGGING
            else
            {
                Debug.LogError($"Require {StatType.MoveSpeed} for this behavior to work!");
                return;
            }
#endif
        }

        public virtual void Update()
        {
            currentRefindTargetTime += Time.deltaTime;
            CheckFindPath();
            if (CheckCanMoveOnPath())
                MoveOnPath();
        }

        public virtual void Disable()
        {
            ClearPath();
            reachedEndOfPath = false;
        }

        protected virtual void PathFoundCompleted(Path newPath)
        {
            pathPositions = newPath.vectorPath;
            reachedEndOfPath = false;
            InitializePath();
        }

        protected virtual void ClearPath()
        {
            currentPathPositionIndex = 1;
            pathPositions = null;
            reachedEndOfPath = false;
        }

        protected virtual void CheckFindPath()
        {
            if (CanFindPath())
            {
                canFindNewPath = false;
                FindNewPath();
            }
        }

        protected virtual bool CanFindPath()
        {
            return canFindNewPath;
        }

        protected abstract void FindNewPath();
        protected abstract void MoveOnPath();
        protected virtual void FinishedMoving()
        {
            ClearPath();
            LockMovement();
            ResetToRefindNewPath();
        }

        protected virtual bool CheckCanMoveOnPath()
            => hasFoundAPath;

        protected virtual void InitializePath()
        {
            reachedEndOfPath = false;

            pathPositions = Helper.Helper.MakeSmoothCurve(pathPositions, 4);

            moveToPosition = pathPositions[pathPositions.Count - 1];
            currentPathPositionIndex = 0;
            hasFoundAPath = true;
        }

        protected virtual void Move()
        {
            if (reachedEndOfPath)
            {
                FinishedMoving();
                return;
            }
            else
            {
                // Update current move;
                if(Vector2.Distance(moveToPosition, PositionData.Position) <= REACH_END_DISTANCE + moveSpeed * Time.deltaTime)
                {
                    reachedEndOfPath = true;
                }
                else
                {
                    var currentPathTargetPosition = pathPositions[currentPathPositionIndex];
                    if (Vector2.Distance(currentPathTargetPosition, PositionData.Position) <= REACH_END_DISTANCE + moveSpeed * Time.deltaTime)
                    {
                        currentPathPositionIndex += 1;
                        ControlData.SetMoveDirection(((Vector2)pathPositions[currentPathPositionIndex] - PositionData.Position).normalized);
                    }
                    else
                    {
                        ControlData.SetMoveDirection(((Vector2)currentPathTargetPosition - PositionData.Position).normalized);
                    }
                }
            }
        }

        protected virtual void ResetToRefindNewPath()
        {
            hasFoundAPath = false;
            canFindNewPath = true;
        }

        protected virtual bool IsObscured()
        {
            float distanceBetweenSqr = Vector2.SqrMagnitude(PositionData.Position - ControlData.Target.Position);
            float currentCheckForwardObscureDistance = CheckObscurationFragmentDistance;
            Vector2 checkForwardDirection = (ControlData.Target.Position - PositionData.Position).normalized;
            Vector2 checkSideDirection = Vector3.Cross(checkForwardDirection, Vector3.forward);

            while (distanceBetweenSqr > currentCheckForwardObscureDistance * currentCheckForwardObscureDistance)
            {
                Vector2 checkForwardObscurePosition = PositionData.Position + checkForwardDirection * currentCheckForwardObscureDistance;
                if (!MapManager.Instance.IsWalkable(checkForwardObscurePosition))
                    return true;

                for (int i = 1; i <= CheckSideObscurationCount; i++)
                {
                    Vector2 checkSideObscurePosition = checkForwardObscurePosition - checkSideDirection * i * CheckObscurationFragmentDistance;
                    if (!MapManager.Instance.IsWalkable(checkSideObscurePosition))
                        return true;
                }

                for (int i = 1; i <= CheckSideObscurationCount; i++)
                {
                    Vector2 checkSideObscurePosition = checkForwardObscurePosition + checkSideDirection * i * CheckObscurationFragmentDistance;
                    if (!MapManager.Instance.IsWalkable(checkSideObscurePosition))
                        return true;
                }

                currentCheckForwardObscureDistance += CheckObscurationFragmentDistance;
            }

            return false;
        }

        protected virtual void LockMovement()
        {
            ControlData.SetMoveDirection(Vector2.zero);
        }

        protected virtual void OnStatChanged(float updatedValue) => moveSpeed = updatedValue;

        public virtual void Dispose() { }

        #endregion Class Methods
    }
}
