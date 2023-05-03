using Cysharp.Threading.Tasks;
using Runtime.Definition;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    [DisallowMultipleComponent]
    public class EntityMoveBehavior : EntityBehavior<IEntityControlData, IEntityStatData>, IUpdateEntityBehavior
    {
        [SerializeField]
        private bool _moveWithRandomSpeed;
        [ShowIf(nameof(_moveWithRandomSpeed))]
        [SerializeField]
        private float _moveRandomOffset;

        private IEntityControlData _controlData;
        private float _moveSpeed;

        protected override UniTask<bool> BuildDataAsync(IEntityControlData controlData, IEntityStatData entityStatData)
        {
            if (entityStatData == null || controlData == null)
                return UniTask.FromResult(false);

            _controlData = controlData;
            _controlData.Position = transform.position;
            _controlData.ForceUpdatePosition = OnForceUpdatePosition;

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
            Vector3 nextPosition = _controlData.Position +  _controlData.MoveDirection.normalized * moveSpeed * deltaTime;
            transform.position = Vector2.MoveTowards(_controlData.Position, nextPosition, moveSpeed * deltaTime);
            _controlData.Position = nextPosition;
        }

        public void OnForceUpdatePosition(Vector2 position)
        {
            transform.position = position;
            _controlData.Position = position;
        }

        private void OnStatChanged(float updatedValue)
        {
            _moveSpeed = updatedValue;
        }
    }

}