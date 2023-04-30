using Cysharp.Threading.Tasks;
using Runtime.Definition;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Runtime.Gameplay.EntitySystem
{
    public class EntityAttackBehavior : EntityBehavior<IEntityControlData>
    {
        private IEntityControlData controlData;

        protected override UniTask<bool> BuildDataAsync(IEntityControlData data)
        {
            controlData.PlayActionEvent += OnTriggerAttack;
            return UniTask.FromResult(true);
        }

        private void OnTriggerAttack(ActionInputType inputType)
        {
            
        }
    }
}
