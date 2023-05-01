using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    [DisallowMultipleComponent]
    public class EntityGetPlayerInputBehavior : EntityBehavior<IEntityControlData>, IUpdateEntityBehavior
    {
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
            var horizontalValue  = Input.GetAxisRaw("Horizontal");
            var verticalValue = Input.GetAxisRaw("Vertical");

            var controlDirection = (new Vector2(horizontalValue, verticalValue)).normalized;
            _controlData.SetMoveDirection(controlDirection);

            if (Input.GetMouseButton(0))
            {
                _controlData.PlayActionEvent.Invoke(Definition.ActionInputType.Attack);
            }
            if (Input.GetMouseButton(1))
            {
                _controlData.PlayActionEvent.Invoke(Definition.ActionInputType.Attack1);
            }
        }
    }
}