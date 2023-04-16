using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public enum UpdateFaceRightType
    {
        MoveDirection,
        FaceDirection,
    }

    public class EntityAnimationBehavior : EntityBehavior<IEntityControlData>, IDisposeEntityBehavior
    {
        [SerializeField] private Transform _flipTransform;
        [SerializeField] private UpdateFaceRightType _updateFaceRightType;
        private IEntityControlData _controlData;
        private IEntityAnimation[] _entityAnimations;

        public void Dispose()
        {
            foreach (var item in _entityAnimations)
                item.Dispose();
        }

        protected override UniTask<bool> BuildDataAsync(IEntityControlData data)
        {
            if (data == null)
                return UniTask.FromResult(false);

            _entityAnimations = GetComponentsInChildren<IEntityAnimation>(true);
            if (_entityAnimations.Length == 0)
                return UniTask.FromResult(false);

            _controlData = data;
            _controlData.MovementChangedEvent += OnMovementChanged;

            if (_updateFaceRightType == UpdateFaceRightType.FaceDirection)
                _controlData.DirectionChangedEvent += OnFaceRightUpdateByFaceDirection;
            else if (_updateFaceRightType == UpdateFaceRightType.MoveDirection)
                _controlData.MovementChangedEvent += OnFaceRightUpdateByMoveDirection;

            foreach (var item in _entityAnimations)
                item.Init(_controlData);

            return UniTask.FromResult(true);
        }

        private void OnMovementChanged()
        {
            if (_controlData.MoveDirection != Vector2.zero)
            {
                PlayerAnimation(AnimationType.Run);
            }    
            else
            {
                PlayerAnimation(AnimationType.Idle);
            }    
        }

        private void OnFaceRightUpdateByFaceDirection()
        {
            if (_controlData.FaceDirection.x > 0)
                _flipTransform.localScale = new Vector2(1, 1);
            else
                _flipTransform.localScale = new Vector2(-1, 1);
        }

        private void OnFaceRightUpdateByMoveDirection()
        {
            if (_controlData.MoveDirection.x > 0)
                _flipTransform.localScale = new Vector2(1, 1);
            else
                _flipTransform.localScale = new Vector2(-1, 1);
        }

        private void PlayerAnimation(AnimationType animationType)
        {
            foreach (var entityAnimation in _entityAnimations)
            {
                entityAnimation.Play(animationType);
            }
        }
    }
}