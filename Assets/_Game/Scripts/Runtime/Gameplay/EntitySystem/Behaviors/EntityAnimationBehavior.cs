using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public class EntityAnimationBehavior : EntityBehavior<IEntityControlData>
    {
        private IEntityControlData _controlData;
        private IEntityAnimation[] _entityAnimations;

        protected override UniTask<bool> BuildDataAsync(IEntityControlData data)
        {
            if (data == null)
                return UniTask.FromResult(false);

            _entityAnimations = GetComponentsInChildren<IEntityAnimation>(true);
            if (_entityAnimations.Length == 0)
                return UniTask.FromResult(false);

            _controlData = data;
            _controlData.MovementChangedEvent += OnMovementChanged;

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

        private void PlayerAnimation(AnimationType animationType)
        {
            foreach (var entityAnimation in _entityAnimations)
            {
                entityAnimation.Play(animationType);
            }
        }
    }
}