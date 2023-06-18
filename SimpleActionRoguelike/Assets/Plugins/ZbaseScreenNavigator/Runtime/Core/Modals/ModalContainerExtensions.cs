using System;

namespace ZBase.UnityScreenNavigator.Core.Modals
{
    public static class ModalContainerExtensions
    {
        /// <summary>
        /// Add callbacks.
        /// </summary>
        /// <param name="self"></param>
        /// <param name="onBeforePush"></param>
        /// <param name="onAfterPush"></param>
        /// <param name="onBeforePop"></param>
        /// <param name="onAfterPop"></param>
        public static void AddCallbackReceiver(this ModalContainer self,
            Action<Modal, Modal, Memory<object>> onBeforePush = null,
            Action<Modal, Modal, Memory<object>> onAfterPush = null,
            Action<Modal, Modal, Memory<object>> onBeforePop = null,
            Action<Modal, Modal, Memory<object>> onAfterPop = null
        )
        {
            var callbackReceiver = new AnonymousModalContainerCallbackReceiver(
                onBeforePush, onAfterPush, onBeforePop, onAfterPop
            );

            self.AddCallbackReceiver(callbackReceiver);
        }

        /// <summary>
        /// Add callbacks.
        /// </summary>
        /// <param name="self"></param>
        /// <param name="modal"></param>
        /// <param name="onBeforePush"></param>
        /// <param name="onAfterPush"></param>
        /// <param name="onBeforePop"></param>
        /// <param name="onAfterPop"></param>
        public static void AddCallbackReceiver(
              this ModalContainer self, Modal modal
            , Action<Modal, Memory<object>> onBeforePush = null, Action<Modal, Memory<object>> onAfterPush = null
            , Action<Modal, Memory<object>> onBeforePop = null, Action<Modal, Memory<object>> onAfterPop = null
        )
        {
            var callbackReceiver = new AnonymousModalContainerCallbackReceiver();

            callbackReceiver.OnBeforePush += (enterModal, exitModal, args) =>
            {
                if (enterModal.Equals(modal))
                {
                    onBeforePush?.Invoke(exitModal, args);
                }
            };

            callbackReceiver.OnAfterPush += (enterModal, exitModal, args) =>
            {
                if (enterModal.Equals(modal))
                {
                    onAfterPush?.Invoke(exitModal, args);
                }
            };

            callbackReceiver.OnBeforePop += (enterModal, exitModal, args) =>
            {
                if (exitModal.Equals(modal))
                {
                    onBeforePop?.Invoke(enterModal, args);
                }
            };

            callbackReceiver.OnAfterPop += (enterModal, exitModal, args) =>
            {
                if (exitModal.Equals(modal))
                {
                    onAfterPop?.Invoke(enterModal, args);
                }
            };

            var gameObj = self.gameObject;

            if (gameObj.TryGetComponent<MonoBehaviourDestroyedEventDispatcher>(out var destroyedEventDispatcher) == false)
            {
                destroyedEventDispatcher = gameObj.AddComponent<MonoBehaviourDestroyedEventDispatcher>();
            }

            destroyedEventDispatcher.OnDispatch += () => self.RemoveCallbackReceiver(callbackReceiver);

            self.AddCallbackReceiver(callbackReceiver);
        }
    }
}