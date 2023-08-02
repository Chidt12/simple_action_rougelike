using Cysharp.Threading.Tasks;
using Runtime.Definition;
using System.Threading;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    [DisallowMultipleComponent]
    public class EntityPhysicMoveBehavior : EntityBehavior<IEntityControlData, IEntityStatData>, IDisposeEntityBehavior
    {
        [SerializeField]
        private Rigidbody2D _rb;
        [SerializeField]
        private Transform _centerTransform;

        private IEntityControlData _controlData;
        private float _moveSpeed;
        private CancellationTokenSource _fixedUpdateTokenSource;

        protected override UniTask<bool> BuildDataAsync(IEntityControlData controlData, IEntityStatData entityStatData)
        {
            if (entityStatData == null || controlData == null)
                return UniTask.FromResult(false);

            _controlData = controlData;
            _controlData.ForceUpdatePosition = OnForceUpdatePosition;

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
                if (_controlData.IsMovable)
                {
                    Vector3 nextPosition = _rb.position + Time.fixedDeltaTime * _controlData.MoveDirection * _moveSpeed;
                    _rb.MovePosition(nextPosition);
                    _controlData.Position = _rb.position;
                    _controlData.CenterPosition = _centerTransform.position;
                }
                await UniTask.Yield(PlayerLoopTiming.FixedUpdate, cancellationToken: _fixedUpdateTokenSource.Token);
            }
        }

        public void OnForceUpdatePosition(Vector2 position, bool ignorePhysic)
        {
            if (ignorePhysic)
            {
                transform.position = position;
                _controlData.Position = position;
                _controlData.CenterPosition = _centerTransform.position;
            }
            else
            {
                _rb.MovePosition(position);
                _controlData.Position = _rb.position;
                _controlData.CenterPosition = _centerTransform.position;
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