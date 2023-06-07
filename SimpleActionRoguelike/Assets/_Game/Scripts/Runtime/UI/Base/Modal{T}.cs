using Cysharp.Threading.Tasks;
using Runtime.Core.Message;
using Runtime.Message;
using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using ZBase.Foundation.PubSub;
using ZBase.UnityScreenNavigator.Core.Modals;

namespace Runtime.UI
{
    public abstract class BaseModal : Modal
    {
        protected int currentSelectedIndex;
        protected List<ISubscription> subscriptions;

        public override UniTask Initialize(Memory<object> args)
        {

            subscriptions = new();
            subscriptions.Add(SimpleMessenger.Subscribe<InputKeyPressMessage>(OnInputKeyPress));
            return base.Initialize(args);
        }

        public override UniTask Cleanup()
        {
            if (subscriptions != null)
            {
                foreach (var subscription in subscriptions)
                    subscription.Dispose();
            }
            return base.Cleanup();
        }

        protected virtual void EnterAButton(Button button)
        {
            if (EventSystem.current != null)
            {
                var ped = new PointerEventData(EventSystem.current);
                ExecuteEvents.Execute(button.gameObject, ped, ExecuteEvents.pointerEnterHandler);
            }
        }

        protected virtual void ExitAButton(Button button)
        {
            if (EventSystem.current != null)
            {
                var ped = new PointerEventData(EventSystem.current);
                ExecuteEvents.Execute(button.gameObject, ped, ExecuteEvents.pointerExitHandler);
            }
        }

        protected virtual void Submit(Button button)
        {
            if (EventSystem.current != null)
            {
                var ped = new PointerEventData(EventSystem.current);
                ExecuteEvents.Execute(button.gameObject, ped, ExecuteEvents.submitHandler);
            }
        }

        protected void OnInputKeyPress(InputKeyPressMessage message)
        {
            if (ScreenNavigator.Instance.IsModalCanDetectAction(this))
            {
                OnKeyPress(message);
            }
        }
        protected virtual void OnKeyPress(InputKeyPressMessage message) { }
    }

    public abstract class Modal<T> : BaseModal where T : class
    {
        public async override UniTask Initialize(Memory<object> args)
        {
            await base.Initialize(args);

            var obj = args.Span[0] as T;

            await Initialize(obj);
        }

        public abstract UniTask Initialize(T data);
    }
}