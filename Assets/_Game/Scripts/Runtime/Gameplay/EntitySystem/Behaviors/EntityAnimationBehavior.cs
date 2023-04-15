using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public class EntityAnimationBehavior : EntityBehavior<IEntityControlData>, IDisposeEntityBehavior
    {
        [SerializeField] private Transform _flipTransform;
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
            _controlData.DirectionChangedEvent += OnDirectionChanged;

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

        private void OnDirectionChanged()
        {
            if (_controlData.FaceDirection.x > 0)
            {
                _flipTransform.localScale = new Vector2(1, 1);
            }
            else
            {
                _flipTransform.localScale = new Vector2(-1, 1);
            }
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