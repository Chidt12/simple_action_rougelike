using Cysharp.Threading.Tasks;
using Runtime.Core.Message;
using Runtime.Message;
using System;
using System.Collections.Generic;
using UnityEngine;
using ZBase.Foundation.PubSub;

namespace Runtime.Gameplay.EntitySystem
{
    [DisallowMultipleComponent]
    public class EntityGetPlayerInputBehavior : EntityBehavior<IEntityControlData>, IDisposeEntityBehavior
    {
        private IEntityControlData _controlData;

        private List<ISubscription> _subscriptions;

        protected override UniTask<bool> BuildDataAsync(IEntityControlData data)
        {
            if (data == null)
                return UniTask.FromResult(false);

            _controlData = data;

            _subscriptions = new();
            _subscriptions.Add(SimpleMessenger.Subscribe<InputMoveVectorMessage>(OnMoveInput));
            _subscriptions.Add(SimpleMessenger.Subscribe<InputAttackMessage>(OnArrowInput));

            return UniTask.FromResult(true);
        }


        public void Dispose()
        {
            foreach (var subscription in _subscriptions)
                subscription.Dispose();
            _subscriptions.Clear();
        }

        private void OnMoveInput(InputMoveVectorMessage message)
        {
            if (!_controlData.IsControllable)
                return;

            var controlDirection = message.MoveVector.normalized;
            _controlData.SetMoveDirection(controlDirection);
        }

        private void OnArrowInput(InputAttackMessage message)
        {
            if (!_controlData.IsControllable)
                return;

            _controlData.PlayActionEvent.Invoke(Definition.ActionInputType.Attack);
        }
    }
}