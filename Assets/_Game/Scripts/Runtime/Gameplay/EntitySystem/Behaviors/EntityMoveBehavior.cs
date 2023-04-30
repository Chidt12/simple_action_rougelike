using Cysharp.Threading.Tasks;
using Runtime.Definition;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public class EntityMoveBehavior : EntityBehavior<IEntityPositionData, IEntityControlData, IEntityStatData>, IUpdateEntityBehavior
    {
        [SerializeField]
        private bool _moveWithRandomSpeed;
        [ShowIf(nameof(_moveWithRandomSpeed))]
        [SerializeField]
        private float _moveRandomOffset;

        private IEntityPositionData _positionData;
        private IEntityControlData _controlData;
        private float _moveSpeed;

        protected override UniTask<bool> BuildDataAsync(IEntityPositionData positionData, IEntityControlData controlData, IEntityStatData entityStatData)
        {
            if (positionData == null || entityStatData == null || controlData == null)
                return UniTask.FromResult(false);

            _positionData = positionData;
            _controlData = controlData;
            _positionData.Position = transform.position;

            if (entityStatData.TryGetStat(StatType.MoveSpeed, out var moveSpeedStat))
            {
                _moveSpeed = moveSpeedStat.TotalValue;
                moveSpeedStat.OnValueChanged += OnStatChanged;
                return UniTask.FromResult(true);
            }
#if UNITY_EDITOR
            else
            {
                Debug.LogError($"Require {StatType.MoveSpeed} for this behavior to work!");
                return UniTask.FromResult(false);
            }
#else
            else
            {
                return UniTask.FromResult(false);
            }
#endif
        }

        public void OnUpdate(float deltaTime)
        {
            var moveSpeed = _moveSpeed;
            if (_moveWithRandomSpeed)
            {
                moveSpeed = Random.Range(_moveSpeed, _moveSpeed + _moveRandomOffset);
            }
            Vector3 nextPosition = _positionData.Position +  _controlData.MoveDirection.normalized * moveSpeed * deltaTime;
            transform.position = Vector2.MoveTowards(_positionData.Position, nextPosition, moveSpeed * deltaTime);
            _positionData.Position = nextPosition;
        }

        private void OnStatChanged(float updatedValue)
        {
            _moveSpeed = updatedValue;
        }
    }

}