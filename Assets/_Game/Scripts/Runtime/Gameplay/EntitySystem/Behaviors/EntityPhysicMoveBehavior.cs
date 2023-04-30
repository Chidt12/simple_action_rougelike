using Cysharp.Threading.Tasks;
using Runtime.Definition;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public class EntityPhysicMoveBehavior : EntityBehavior<IEntityPositionData, IEntityControlData, IEntityStatData>, IDisposeEntityBehavior
    {
        [SerializeField]
        private Rigidbody2D _rb;

        private IEntityPositionData _positionData;
        private IEntityControlData _controlData;
        private float _moveSpeed;
        private CancellationTokenSource _fixedUpdateTokenSource;

        protected override UniTask<bool> BuildDataAsync(IEntityPositionData positionData, IEntityControlData controlData, IEntityStatData entityStatData)
        {
            if (positionData == null || entityStatData == null || controlData == null)
                return UniTask.FromResult(false);

            _positionData = positionData;
            _controlData = controlData;

            if (entityStatData.TryGetStat(StatType.MoveSpeed, out var moveSpeedStat))
            {
                _moveSpeed = moveSpeedStat.TotalValue;
                moveSpeedStat.OnValueChanged += OnStatChanged;
                _fixedUpdateTokenSource = new CancellationTokenSource();
                StartFixedUpdateAsync().Forget();
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

        private async UniTaskVoid StartFixedUpdateAsync()
        {
            while (true)
            {
                Vector3 nextPosition = _rb.position + Time.fixedDeltaTime * _controlData.MoveDirection * _moveSpeed;
                _rb.MovePosition(nextPosition);
                _positionData.Position = _rb.position;
                await UniTask.Yield(PlayerLoopTiming.FixedUpdate, cancellationToken: _fixedUpdateTokenSource.Token);
            }
        }

        private void OnStatChanged(float updatedValue)
        {
            _moveSpeed = updatedValue;
        }

        public void Dispose()
        {
            _fixedUpdateTokenSource.Cancel();
        }
    }

}