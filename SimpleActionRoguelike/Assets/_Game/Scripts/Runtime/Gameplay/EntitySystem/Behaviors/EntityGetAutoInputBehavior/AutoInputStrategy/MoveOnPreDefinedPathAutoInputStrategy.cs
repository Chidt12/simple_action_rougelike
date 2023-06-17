using PathCreation;
using Runtime.Definition;
using Runtime.Manager.Gameplay;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public class MoveOnPreDefinedPathAutoInputStrategy : IAutoInputStrategy
    {
        protected IEntityControlData ControlData { get; private set; }
        protected IEntityControlCastRangeProxy ControlCastRangeProxy { get; private set; }

        public EndOfPathInstruction endOfPathInstruction;
        protected float moveSpeed;
        private PathCreator _pathCreator;
        private float _distanceTravelled;
        private EndOfPathInstruction _endOfPathInstruction;

        public MoveOnPreDefinedPathAutoInputStrategy(IEntityControlData controlData, IEntityStatData statData, IEntityControlCastRangeProxy entityControlCastRangeProxy)
        {
            ControlData = controlData;
            ControlCastRangeProxy = entityControlCastRangeProxy;
            if(MapManager.Instance.PathCreators.Length > 0)
                _pathCreator = MapManager.Instance.PathCreators[Random.Range(0, MapManager.Instance.PathCreators.Length)];

            if (statData.TryGetStat(StatType.MoveSpeed, out var statSpeed))
            {
                moveSpeed = statSpeed.TotalValue;
                statSpeed.OnValueChanged += OnStatChanged;
            }

            _distanceTravelled = 0;

#if DEBUGGING
            else
            {
                Debug.LogError($"Require {StatType.MoveSpeed} for this behavior to work!");
                return;
            }
#endif
        }

        public void Dispose()
        {}

        public void Update()
        {
            if (_pathCreator == null)
                return;

            //_distanceTravelled = _pathCreator.path.GetClosestDistanceAlongPath(ControlData.Position);
            _distanceTravelled += moveSpeed * Time.deltaTime;

            if (!_pathCreator.path.isClosedLoop)
            {
                _endOfPathInstruction = EndOfPathInstruction.Reverse;
            }
            else
            {
                _endOfPathInstruction = EndOfPathInstruction.Loop;
            }

            var nextPosition = _pathCreator.path.GetPointAtDistance(_distanceTravelled, _endOfPathInstruction);
            ControlData.SetMoveDirection((Vector2)nextPosition - ControlData.Position);
        }

        protected void OnStatChanged(float updatedValue) => moveSpeed = updatedValue;
    }
}