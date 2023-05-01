using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public enum FaceDirectionType
    {
        Target,
        MoveDirection,
        Pointer,
    }

    [DisallowMultipleComponent]
    public class EntityUpdateFaceDirectionBehavior : EntityBehavior<IEntityControlData>, IUpdateEntityBehavior
    {
        [SerializeField] private FaceDirectionType _faceDirectionType;
        private IEntityControlData _controlData;

        protected override UniTask<bool> BuildDataAsync(IEntityControlData data)
        {
            if (data == null)
                return UniTask.FromResult(false);

            _controlData = data;
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
                case FaceDirectionType.Pointer:
                    var currentPointerPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    Vector2 centerToPointer = currentPointerPosition - transform.position;
                    _controlData.SetFaceDirection(centerToPointer);
                    break;
                default:
                    break;
            }
        }
    }
}