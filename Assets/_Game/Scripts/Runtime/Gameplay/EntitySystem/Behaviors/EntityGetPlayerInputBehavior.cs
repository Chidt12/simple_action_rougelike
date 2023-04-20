using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
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

            if (Input.GetMouseButtonDown(0))
            {
                _controlData.TriggerAttack.Invoke(0);
            }
            if (Input.GetMouseButtonDown(1))
            {
                _controlData.TriggerAttack.Invoke(1);
            }
        }
    }
}