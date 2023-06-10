using Cysharp.Threading.Tasks;
using Runtime.Core.Message;
using Runtime.Message;
using System;
using System.Collections.Generic;
using UnityEngine;
using ZBase.Foundation.PubSub;

namespace Runtime.Gameplay.EntitySystem
{
    public enum PlayerAttackInputType
    {
        FourDirection = 0,
        PointerClick = 1,
    }

    [DisallowMultipleComponent]
    public class EntityGetPlayerInputBehavior : EntityBehavior<IEntityControlData>, IDisposeEntityBehavior
    {
        [SerializeField]
        private PlayerAttackInputType _playerAttackInputType;

        private IEntityControlData _controlData;
        private List<ISubscription> _subscriptions;

        protected override UniTask<bool> BuildDataAsync(IEntityControlData data)
        {
            if (data == null)
                return UniTask.FromResult(false);

            _controlData = data;

            _subscriptions = new();
            _subscriptions.Add(SimpleMessenger.Subscribe<InputMoveVectorMessage>(OnMoveInput));
            _subscriptions.Add(SimpleMessenger.Subscribe<InputKeyPressMessage>(OnKeyPressInput));

            if (_playerAttackInputType == PlayerAttackInputType.FourDirection)
            {
                _subscriptions.Add(SimpleMessenger.Subscribe<InputAttackMessage>(OnArrowInput));
            }


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

        private void OnKeyPressInput(InputKeyPressMessage message)
        {
            if (!_controlData.IsControllable)
                return;

            if (message.KeyPressType == KeyPressType.LeftMouseButton && _playerAttackInputType == PlayerAttackInputType.PointerClick)
                _controlData.PlayActionEvent.Invoke(Definition.ActionInputType.Attack);
            else if (message.KeyPressType == KeyPressType.Dash)
                _controlData.PlayActionEvent.Invoke(Definition.ActionInputType.Dash);
        }
    }
}