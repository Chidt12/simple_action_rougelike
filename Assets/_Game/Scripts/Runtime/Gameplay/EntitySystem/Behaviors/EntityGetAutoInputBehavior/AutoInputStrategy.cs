using Pathfinding;
using Pathfinding.RVO;
using Pathfinding.Util;
using Runtime.Manager.Gameplay;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    // Follow Script AIPath.
    public abstract class AutoInputStrategy : IAutoInputStrategy
    {
        #region Members

        protected const int PATH_FINDING_COST_MULTIPLIER = 1000;
        protected static readonly float Radius = 0.3f;
        protected static readonly float Height = 2.0f;
        protected static readonly float MaxAcceleration = -10.0f;
        protected static readonly float RotationSpeed = 360.0f;
        protected static readonly float SlowdownDistance = 0.6f;
        protected static readonly float PickNextWaypointDistance = 0.25f;
        protected static readonly bool ConstrainInsideGraph = true;
        protected static readonly float EndReachedDistance = 0.2f;

        protected static readonly float RefindTargetBonusRange = 3.0f;
        protected static readonly float RefindTargetMinTime = 0.5f;
        protected static readonly int RandomMoveSearchSlotsCount = 3;
        protected static readonly int RandomMoveSearchSpreadSlotsCount = 2;
        protected static readonly float CheckObscurationFragmentDistance = 1 / 5.0f;
        protected static readonly float CheckSideObscurationCount = 0;

        protected bool canFindNewPath;
        protected bool hasFoundAPath;
        protected float moveSpeed;
        protected float stopChasingTargetDistanceSqr;
        protected float stopChasingTargetDistance;
        protected float refindTargetThresholdSqr;
        protected float currentRefindTargetTime;
        protected int randomMoveSearchLength;
        protected int randomMoveSearchSpreadLength;
        protected Vector2 pathTargetPosition;
        protected Vector2 moveToPosition;
        protected List<Vector3> pathPositions;
        protected Vector2 simulatedPosition;
        protected Vector2 velocity2D;
        protected float lastDeltaTime;
        protected int previousFrame;
        protected Vector3 previousPosition1;
        protected Vector3 previousPosition2;
        protected Vector2 lastDeltaPosition;
        protected Vector3 accumulatedMovementDelta;
        protected Vector3 nextPosition;
        protected Quaternion nextRotation;
        protected PathInterpolator interpolator = new PathInterpolator();
        protected IMovementPlane movementPlane = GraphTransform.identityTransform;
        protected CustomRVOController rvoController;

        // Custom parameters
        protected IEntityPositionData target;

        #endregion Members

        #region Properties

        public Quaternion rotation { get; set; }
        public Vector3 position { get { return PositionData.Position; } }
        public Vector3 destination { get; set; }
        public Vector3 velocity { get { return lastDeltaTime > 0.000001f ? (previousPosition1 - previousPosition2) / lastDeltaTime : Vector3.zero; } }
        public Vector3 desiredVelocity { get { return lastDeltaTime > 0.00001f ? movementPlane.ToWorld(lastDeltaPosition / lastDeltaTime) : Vector3.zero; } }
        public bool isStopped { get; set; }
        public Action onSearchPath { get; set; }
        public bool reachedEndOfPath { get; protected set; }
        public bool hasPath { get { return interpolator.valid; } }
        public bool pathPending { get; }
        public Vector3 steeringTarget { get { return interpolator.valid ? interpolator.position : position; } }

        public float remainingDistance
        {
            get
            {
                return interpolator.valid ? interpolator.remainingDistance + movementPlane.ToPlane(interpolator.position - position).magnitude
                                          : float.PositiveInfinity;
            }
        }

        public bool reachedDestination
        {
            get
            {
                if (!reachedEndOfPath)
                    return false;

                if (!interpolator.valid || remainingDistance > EndReachedDistance)
                    return false;

                return true;
            }
        }
        protected IEntityControlData ControlData { get; private set; }
        protected IEntityPositionData PositionData { get; private set; }
        float IAstarAI.radius { get; set; } = Radius;
        float IAstarAI.height { get; set; } = Height;
        float IAstarAI.maxSpeed { get { return moveSpeed; } set { moveSpeed = value; } }
        bool IAstarAI.canSearch { get; set; }
        bool IAstarAI.canMove { get; set; }

        #endregion Properties

        #region Class Methods

        public AutoInputStrategy(IEntityPositionData positionData, IEntityControlData controlData, IEntityStatData statData, float castRange, CustomRVOController rvoController)
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
            currentRefindTargetTime = 0.0f;

            this.rvoController = rvoController;
            this.rvoController?.Init(this);
            Teleport(PositionData.Position, false);

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
            CheckFindPath();
            if (CheckCanMoveOnPath())
                MoveOnPath();
        }

        public virtual void Disable()
        {
            ClearPath();
            velocity2D = Vector3.zero;
            accumulatedMovementDelta = Vector3.zero;
            lastDeltaTime = 0;
            reachedEndOfPath = false;
        }

        public virtual void Teleport(Vector3 newPosition, bool clearPath = true)
        {
            reachedEndOfPath = false;

            if (clearPath)
                ClearPath();

            previousPosition1 = previousPosition2 = simulatedPosition = newPosition;
            PositionData.Position = newPosition;

            if (rvoController != null)
                rvoController.Move(Vector3.zero);

            if (clearPath)
                SearchPath();
        }

        public virtual void SearchPath() { }
        public void SetPath(Path path, bool updateDestinationFromPath = true) { }

        public void MovementUpdate(float deltaTime, out Vector3 nextPosition, out Quaternion nextRotation)
        {
            lastDeltaTime = deltaTime;
            MovementUpdateInternal(deltaTime, out nextPosition, out nextRotation);
        }

        public virtual void Move(Vector3 deltaPosition)
            => accumulatedMovementDelta += deltaPosition;

        public void GetRemainingPath(List<Vector3> buffer, out bool stale)
        {
            buffer.Clear();
            buffer.Add(position);
            if (!interpolator.valid)
            {
                stale = true;
                return;
            }
            stale = false;
            interpolator.GetRemainingPath(buffer);
        }

        public virtual void FinalizeMovement(Vector3 nextPosition, Quaternion nextRotation)
        {
            Vector3 currentPosition = simulatedPosition;
            float lastElevation;
            movementPlane.ToPlane(currentPosition, out lastElevation);
            currentPosition = nextPosition + accumulatedMovementDelta;
            currentPosition = ClampToNavmesh(currentPosition);
            accumulatedMovementDelta = Vector3.zero;
            simulatedPosition = currentPosition;
            UpdateVelocity();
        }

        protected Vector2 CalculateDeltaToMoveThisFrame(Vector2 position, float distanceToEndOfPath, float deltaTime)
        {
            if (rvoController != null && rvoController.enabled)
                return movementPlane.ToPlane(rvoController.CalculateMovementDelta(movementPlane.ToWorld(position, 0), deltaTime));
            else
                return Vector2.ClampMagnitude(velocity2D * deltaTime, distanceToEndOfPath);
        }

        protected virtual void UpdateVelocity()
        {
            var currentFrame = Time.frameCount;
            if (currentFrame != previousFrame)
                previousPosition2 = previousPosition1;
            previousPosition1 = position;
            previousFrame = currentFrame;
        }

        protected virtual void PathFoundCompleted(Path newPath)
        {
            pathPositions = newPath.vectorPath;
            interpolator.SetPath(pathPositions);
            var graph = newPath.path.Count > 0 ? AstarData.GetGraph(newPath.path[0]) as ITransformedGraph : null;
            movementPlane = graph != null ? graph.transform : new GraphTransform(Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(-90, 270, 90), Vector3.one));
            reachedEndOfPath = false;
            interpolator.MoveToLocallyClosestPoint(position);
            interpolator.MoveToCircleIntersection2D(position, PickNextWaypointDistance, movementPlane);
            InitializePath();
        }

        protected virtual void ClearPath()
        {
            pathPositions = null;
            reachedEndOfPath = false;
            interpolator.SetPath(null);
        }

        protected virtual void MovementUpdateInternal(float deltaTime, out Vector3 nextPosition, out Quaternion nextRotation)
        {
            float currentAcceleration = MaxAcceleration;
            if (currentAcceleration < 0)
                currentAcceleration *= -moveSpeed;

            simulatedPosition = PositionData.Position;
            Vector3 currentPosition = simulatedPosition;
            interpolator.MoveToCircleIntersection2D(currentPosition, PickNextWaypointDistance, movementPlane);
            var direction = movementPlane.ToPlane(steeringTarget - currentPosition);
            float distanceToEnd = remainingDistance;
            reachedEndOfPath = distanceToEnd <= EndReachedDistance && interpolator.valid;

            float slowdown;
            var forwards = movementPlane.ToPlane(Vector3.up);
            bool stopped = isStopped || reachedDestination;
            if (interpolator.valid && !stopped)
            {
                slowdown = distanceToEnd < SlowdownDistance ? Mathf.Sqrt(distanceToEnd / SlowdownDistance) : 1;
                if (reachedEndOfPath)
                    velocity2D -= Vector2.ClampMagnitude(velocity2D, currentAcceleration * deltaTime);
                else
                    velocity2D += MovementUtilities.CalculateAccelerationToReachPoint(direction, direction.normalized * moveSpeed, velocity2D, currentAcceleration, RotationSpeed, moveSpeed, forwards) * deltaTime;
            }
            else
            {
                slowdown = 1;
                velocity2D -= Vector2.ClampMagnitude(velocity2D, currentAcceleration * deltaTime);
            }

            velocity2D = MovementUtilities.ClampVelocity(velocity2D, moveSpeed, slowdown, false, forwards);
            if (rvoController != null && rvoController.enabled)
            {
                var rvoTarget = currentPosition + movementPlane.ToWorld(Vector2.ClampMagnitude(velocity2D, distanceToEnd), 0f);
                rvoController.SetTarget(rvoTarget, velocity2D.magnitude, moveSpeed);
            }

            var delta2D = lastDeltaPosition = CalculateDeltaToMoveThisFrame(movementPlane.ToPlane(currentPosition), distanceToEnd, deltaTime);
            nextPosition = currentPosition + movementPlane.ToWorld(delta2D);
            nextRotation = Quaternion.identity;
        }

        protected virtual Vector3 ClampToNavmesh(Vector3 position)
        {
            if (ConstrainInsideGraph)
            {
                var clampedPosition = AstarPath.active.GetNearest(position, NNConstraint.Default).position;
                var difference = movementPlane.ToPlane(clampedPosition - position);
                float sqrDifference = difference.sqrMagnitude;
                // Don't check for equality because some precision may be lost if any coordinate transformations are used.
                if (sqrDifference > 0.001f * 0.001f)
                {
                    // The agent was outside the navmesh. Remove that component of the velocity so that the velocity only goes along the direction of the wall, not into it.
                    velocity2D -= difference * Vector2.Dot(difference, velocity2D) / sqrDifference;

                    // Make sure the RVO system knows that there was a collision here, otherwise other agents may think this
                    // agent continued to move forwards and avoidance quality may suffer.
                    if (rvoController != null && rvoController.enabled)
                        rvoController.SetCollisionNormal(difference);

                    return position + movementPlane.ToWorld(difference);
                }
            }

            return position;
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
            RefindNewPath();
        }

        protected virtual bool CheckCanMoveOnPath()
            => hasFoundAPath;

        protected virtual void InitializePath()
        {
            reachedEndOfPath = false;
            pathTargetPosition = PositionData.Position;
            moveToPosition = pathPositions[pathPositions.Count - 1];
            hasFoundAPath = true;
        }

        protected virtual void Move()
        {
            currentRefindTargetTime += Time.deltaTime;
            if (reachedEndOfPath)
            {
                FinishedMoving();
                return;
            }
            else
            {
                MovementUpdate(Time.deltaTime, out nextPosition, out nextRotation);
                FinalizeMovement(nextPosition, nextRotation);
                var moveDelta = simulatedPosition - PositionData.Position;
                ControlData.SetMoveDelta(moveDelta);
            }
        }

        protected virtual void RefindNewPath()
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
            ControlData.SetMoveDelta(Vector2.zero);
        }

        protected virtual void OnStatChanged(float updatedValue) => moveSpeed = updatedValue;

        public void SetPath(Path path) { }
        public virtual void Dispose() { }

        #endregion Class Methods
    }
}
