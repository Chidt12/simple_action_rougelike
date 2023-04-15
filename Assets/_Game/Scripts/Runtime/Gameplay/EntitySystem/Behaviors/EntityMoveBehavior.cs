using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public class EntityMoveBehavior : EntityBehavior<IEntityPositionData, IEntityStatData>, IUpdateEntityBehavior
    {
        private IEntityPositionData _positionData;
        private float _moveSpeed;

        protected override UniTask<bool> BuildDataAsync(IEntityPositionData positionData, IEntityStatData entityStatData)
        {
            if (positionData == null || entityStatData == null)
                return UniTask.FromResult(false);

            _positionData = positionData;
            var tryGetMoveSpeed = 0.0f;
            if (entityStatData.TryGetStat(StatType.MoveSpeed, out var moveSpeedStat))
            {
                _moveSpeed = tryGetMoveSpeed;
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
            Vector3 nextPosition = _positionData.Position;
        }

        private void OnStatChanged(float updatedValue)
        {
            _moveSpeed = updatedValue;
        }
    }

}