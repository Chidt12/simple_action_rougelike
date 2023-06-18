using System;

namespace ZBase.UnityScreenNavigator.Core.Modals
{
    public sealed class AnonymousModalContainerCallbackReceiver : IModalContainerCallbackReceiver
    {
        public event Action<Modal, Modal, Memory<object>> OnAfterPop;
        public event Action<Modal, Modal, Memory<object>> OnAfterPush;
        public event Action<Modal, Modal, Memory<object>> OnBeforePop;
        public event Action<Modal, Modal, Memory<object>> OnBeforePush;

        public AnonymousModalContainerCallbackReceiver(
              Action<Modal, Modal, Memory<object>> onBeforePush = null
            , Action<Modal, Modal, Memory<object>> onAfterPush = null
            , Action<Modal, Modal, Memory<object>> onBeforePop = null
            , Action<Modal, Modal, Memory<object>> onAfterPop = null
        )
        {
            OnBeforePush = onBeforePush;
            OnAfterPush = onAfterPush;
            OnBeforePop = onBeforePop;
            OnAfterPop = onAfterPop;
        }

        void IModalContainerCallbackReceiver.BeforePush(Modal enterModal, Modal exitModal, Memory<object> args)
        {
            OnBeforePush?.Invoke(enterModal, exitModal, args);
        }

        void IModalContainerCallbackReceiver.AfterPush(Modal enterModal, Modal exitModal, Memory<object> args)
        {
            OnAfterPush?.Invoke(enterModal, exitModal, args);
        }

        void IModalContainerCallbackReceiver.BeforePop(Modal enterModal, Modal exitModal, Memory<object> args)
        {
            OnBeforePop?.Invoke(enterModal, exitModal, args);
        }

        void IModalContainerCallbackReceiver.AfterPop(Modal enterModal, Modal exitModal, Memory<object> args)
        {
            OnAfterPop?.Invoke(enterModal, exitModal, args);
        }
    }
}