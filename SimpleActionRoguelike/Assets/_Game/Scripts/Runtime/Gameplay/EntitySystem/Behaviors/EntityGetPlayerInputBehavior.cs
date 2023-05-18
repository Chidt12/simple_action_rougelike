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
            if (!_controlData.IsControllable)
                return;

            var horizontalValue  = Input.GetAxisRaw("Horizontal");
            var verticalValue = Input.GetAxisRaw("Vertical");

            var controlDirection = (new Vector2(horizontalValue, verticalValue)).normalized;
            _controlData.SetMoveDirection(controlDirection);

            // Fight for just 4 direction.
            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.UpArrow))
            {
                _controlData.PlayActionEvent.Invoke(Definition.ActionInputType.Attack);
            }
        }
    }
}