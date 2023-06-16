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

        protected bool reachedEndOfPath;
        protected bool findingNewPath;
        protected bool hasFoundAPath;
        protected float moveSpeed;
        protected float StopChasingTargetDistance => ControlCastRangeProxy.CastRange;
        protected float RefindTargetThreshold => StopChasingTargetDistance + RefindTargetBonusRange;

        protected virtual float RefindTargetMinTime => 2f;

        protected float currentRefindTargetTime;
        protected List<Vector3> pathPositions;
        protected Vector2 moveToPosition;
        protected int currentPathPositionIndex;

        #endregion Members

        #region Properties
        protected IEntityControlData ControlData { get; private set; }
        protected IEntityControlCastRangeProxy ControlCastRangeProxy { get; private set; }

        #endregion Properties

        #region Class Methods

        public AutoInputStrategy(IEntityControlData controlData, IEntityStatData statData, IEntityControlCastRangeProxy entityControlCastRangeProxy)
        {
            ControlData = controlData;
            ControlCastRangeProxy = entityControlCastRangeProxy;
            findingNewPath = false;
            hasFoundAPath = false;
            currentRefindTargetTime = RefindTargetMinTime;

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

            if (CanFindPath())
            {
                findingNewPath = true;
                FindNewPath();
            }

            if (hasFoundAPath)
                MoveOnPath();
        }

        protected virtual bool CanFindPath()
        {
            return !findingNewPath && currentRefindTargetTime > RefindTargetMinTime;
        }

        protected abstract void FindNewPath();
        protected abstract void MoveOnPath();

        protected virtual void ReachedTheEndOfPath()
        {
            LockMovement();
            ResetToRefindNewPath();
        }

        protected virtual void Move()
        {
            if (reachedEndOfPath)
            {
                ReachedTheEndOfPath();
                return;
            }
            else
            {
                // Update current move;
                if(Vector2.Distance(moveToPosition, ControlData.Position) <= REACH_END_DISTANCE + moveSpeed * Time.deltaTime)
                {
                    reachedEndOfPath = true;
                }
                else
                {
                    var currentPathTargetPosition = pathPositions[currentPathPositionIndex];
                    if (Vector2.Distance(currentPathTargetPosition, ControlData.Position) <= REACH_END_DISTANCE + moveSpeed * Time.deltaTime)
                    {
                        currentPathPositionIndex += 1;
                        ControlData.SetMoveDirection(((Vector2)pathPositions[currentPathPositionIndex] - ControlData.Position).normalized);
                    }
                    else
                    {
                        ControlData.SetMoveDirection(((Vector2)currentPathTargetPosition - ControlData.Position).normalized);
                    }
                }
            }
        }

        protected virtual void ResetToRefindNewPath()
        {
            hasFoundAPath = false;
            findingNewPath = false;
        }

        protected void LockMovement() => ControlData.SetMoveDirection(Vector2.zero);

        protected virtual void PathFoundCompleted(List<Vector3> positions)
        {
            reachedEndOfPath = false;
            pathPositions = positions;
            pathPositions = Helper.Helper.MakeSmoothCurve(pathPositions, 4);
            moveToPosition = pathPositions[pathPositions.Count - 1];
            currentPathPositionIndex = 0;
            hasFoundAPath = true;
        }

        protected void OnStatChanged(float updatedValue) => moveSpeed = updatedValue;
        public virtual void Dispose() { }

        #endregion Class Methods
    }
}
