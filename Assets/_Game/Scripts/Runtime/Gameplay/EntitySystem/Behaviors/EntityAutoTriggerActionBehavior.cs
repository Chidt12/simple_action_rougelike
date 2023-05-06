using Cysharp.Threading.Tasks;
using Runtime.Definition;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    [DisallowMultipleComponent]
    public class EntityAutoTriggerActionBehavior : EntityBehavior<IEntityControlData>, IUpdateEntityBehavior
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

            _controlData.PlayActionEvent.Invoke(ActionInputType.UseSkill1);
        }
    }
}
