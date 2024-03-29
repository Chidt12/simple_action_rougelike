using Runtime.Definition;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public abstract class AutoInputStrategy : IAutoInputStrategy
    {
        #region Members

        protected const int PATH_FINDING_COST_MULTIPLIER = 1000;
        protected const float REACH_END_DISTANCE = 0.2f;
        protected static readonly float RefindTargetBonusRange = 2.0f;
        protected static float OFFSET_TIME_TO_FORCE_FIND_PATH = 2;

        protected bool reachedEndOfPath;
        protected bool findingNewPath;
        protected bool hasFoundAPath;
        protected float moveSpeed;
        protected float StopChasingTargetDistance => ControlCastRangeProxy.CastRange;
        protected float RefindTargetThreshold => StopChasingTargetDistance + RefindTargetBonusRange;

        protected virtual float RefindTargetMinTime => 2f;

        protected float currentForceRefindTime;
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

            ControlData.ForceUpdatePathEvent += OnForceUpdatePath;

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

        private void OnForceUpdatePath()
        {
            currentRefindTargetTime = RefindTargetMinTime;
            hasFoundAPath = false;
            findingNewPath = false;
            LockMovement();
        }

        public virtual void Update()
        {
            currentRefindTargetTime += Time.deltaTime;

            if (CanFindPath())
            {
                Debug.LogWarning("Start Find Path " + ControlData.EntityUID);
                findingNewPath = true;
                currentForceRefindTime = OFFSET_TIME_TO_FORCE_FIND_PATH;
                FindNewPath();
            }
            else
            {
                currentForceRefindTime -= Time.deltaTime;
            }

            if (hasFoundAPath)
                MoveOnPath();
        }

        protected virtual bool CanFindPath()
        {
            return (!findingNewPath || currentForceRefindTime < 0 ) && !hasFoundAPath && currentRefindTargetTime > RefindTargetMinTime;
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

        protected virtual void ResetToRefindNewPath(bool keepMovingOnPath = false)
        {
            if(!keepMovingOnPath)
                hasFoundAPath = false;
            findingNewPath = false;
        }

        protected void LockMovement() => ControlData.SetMoveDirection(Vector2.zero);

        protected virtual void PathFoundCompleted(List<Vector3> positions, bool makeSmooth = true)
        {
            findingNewPath = false;
            reachedEndOfPath = false;
            pathPositions = positions;
            if(makeSmooth)
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
