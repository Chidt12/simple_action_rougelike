using Runtime.Definition;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public class MoveByWayAutoInputStrategy : IAutoInputStrategy
    {
        protected const float REACH_END_DISTANCE = 0.2f;
        protected static readonly float RefindTargetBonusRange = 2.0f;
        protected bool reachedEndOfPath;
        protected float moveSpeed;

        protected IEntityControlData ControlData { get; private set; }
        protected IEntityControlCastRangeProxy ControlCastRangeProxy { get; private set; }

        protected float currentRefindTargetTime;
        protected List<Vector3> pathPositions;
        protected Vector2 moveToPosition;
        protected int currentPathPositionIndex;

        public MoveByWayAutoInputStrategy(IEntityControlData controlData, IEntityStatData statData, IEntityControlCastRangeProxy entityControlCastRangeProxy)
        {
            ControlData = controlData;
            ControlCastRangeProxy = entityControlCastRangeProxy;

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

        public void Update()
        {
            // Logic Move horizontal first => then vertical => check by ray cast.
        }

        public void Dispose()
        {
            
        }

        protected void OnStatChanged(float updatedValue) => moveSpeed = updatedValue;
    }
}