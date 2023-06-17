using PathCreation;
using Runtime.Manager.Gameplay;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Runtime.Gameplay.EntitySystem
{
    public class MoveOnPreDefinedPathFollowTargetAutoInputStrategy : AutoInputStrategy
    {
        private PathCreator _pathCreator;

        public MoveOnPreDefinedPathFollowTargetAutoInputStrategy(IEntityControlData controlData, IEntityStatData statData, IEntityControlCastRangeProxy entityControlCastRangeProxy) 
            : base(controlData, statData, entityControlCastRangeProxy)
        {
            if (MapManager.Instance.PathCreators.Length > 0)
                _pathCreator = MapManager.Instance.PathCreators[Random.Range(0, MapManager.Instance.PathCreators.Length)];
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
                && !ControlData.Target.IsDead && _pathCreator != null;
        }

        protected override void FindNewPath()
        {
            var indexPointOfTarget = _pathCreator.path.GetClosestIndexPointOnLocalPointIndex(ControlData.Target.Position);
            var indexPointOfCreator = _pathCreator.path.GetClosestIndexPointOnLocalPointIndex(ControlData.Position);

            if(indexPointOfTarget == indexPointOfCreator)
            {
                findingNewPath = false;
            }
            else
            {
                var movePoints = new List<Vector3>();

                if(indexPointOfCreator < indexPointOfTarget)
                {
                    for (int i = indexPointOfCreator; i <= indexPointOfTarget; i++)
                    {
                        movePoints.Add(_pathCreator.path.GetPoint(i));
                    }

                    PathFoundCompleted(movePoints);
                }
                else
                {
                    for (int i = indexPointOfCreator; i >= indexPointOfTarget; i--)
                    {
                        movePoints.Add(_pathCreator.path.GetPoint(i));
                    }

                    PathFoundCompleted(movePoints);
                }
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