using Cysharp.Threading.Tasks;
using Runtime.Core.Message;
using Runtime.Message;
using System.Collections.Generic;
using UnityEngine;
using ZBase.Foundation.PubSub;

namespace Runtime.Gameplay.EntitySystem
{
    public enum FaceDirectionType
    {
        Target,
        MoveDirection,
        Pointer,
        FourDirection,
    }

    [DisallowMultipleComponent]
    public class EntityUpdateFaceDirectionBehavior : EntityBehavior<IEntityControlData>, IUpdateEntityBehavior, IDisposeEntityBehavior
    {
        [SerializeField] private FaceDirectionType _faceDirectionType;
        
        private IEntityControlData _controlData;
        private List<ISubscription> _subscriptions;

        protected override UniTask<bool> BuildDataAsync(IEntityControlData data)
        {
            if (data == null)
                return UniTask.FromResult(false);

            _controlData = data;
            
            if(_faceDirectionType == FaceDirectionType.FourDirection)
            {
                _subscriptions = new();
                _subscriptions.Add(SimpleMessenger.Subscribe<InputAttackMessage>(OnInputAttack));
            }

            return UniTask.FromResult(true);
        }

        public void OnUpdate(float deltaTime)
        {
            switch (_faceDirectionType)
            {
                case FaceDirectionType.Target:
                    if(_controlData.Target != null && !_controlData.Target.IsDead)
                    {
                        Vector2 faceDirection = _controlData.Target.Position - (Vector2)transform.position;
                        _controlData.SetFaceDirection(faceDirection);
                    }
                    else
                    {
                        _controlData.SetFaceDirection(_controlData.MoveDirection);
                    }
                    break;
                case FaceDirectionType.MoveDirection:
                    _controlData.SetFaceDirection(_controlData.MoveDirection);
                    break;
                default:
                    break;
            }
        }

        private void OnInputAttack(InputAttackMessage message)
        {
            switch (message.ArrowType)
            {
                case InputAttackType.Right:
                    _controlData.SetFaceDirection(Vector2.right);
                    break;
                case InputAttackType.Left:
                    _controlData.SetFaceDirection(Vector2.left);
                    break;
                case InputAttackType.Up:
                    _controlData.SetFaceDirection(Vector2.up);
                    break;
                case InputAttackType.Down:
                    _controlData.SetFaceDirection(Vector2.down);
                    break;
                default:
                    break;
            }
        }

        public void Dispose()
        {
            if(_subscriptions != null)
            {
                foreach (var subscription in _subscriptions)
                    subscription.Dispose();

                _subscriptions.Clear();
            }
        }
    }
}