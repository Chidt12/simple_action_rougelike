using System;

namespace ZBase.UnityScreenNavigator.Core.Screens
{
    public static class ScreenContainerExtensions
    {
        /// <summary>
        /// Add callbacks.
        /// </summary>
        /// <param name="self"></param>
        /// <param name="onBeforePush"></param>
        /// <param name="onAfterPush"></param>
        /// <param name="onBeforePop"></param>
        /// <param name="onAfterPop"></param>
        public static void AddCallbackReceiver(
              this ScreenContainer self
            , Action<Screen, Screen, Memory<object>> onBeforePush = null
            , Action<Screen, Screen, Memory<object>> onAfterPush = null
            , Action<Screen, Screen, Memory<object>> onBeforePop = null
            , Action<Screen, Screen, Memory<object>> onAfterPop = null
        )
        {
            var callbackReceiver = new AnonymousScreenContainerCallbackReceiver(
                onBeforePush, onAfterPush, onBeforePop, onAfterPop
            );

            self.AddCallbackReceiver(callbackReceiver);
        }

        /// <summary>
        /// Add callbacks.
        /// </summary>
        /// <param name="self"></param>
        /// <param name="screen"></param>
        /// <param name="onBeforePush"></param>
        /// <param name="onAfterPush"></param>
        /// <param name="onBeforePop"></param>
        /// <param name="onAfterPop"></param>
        public static void AddCallbackReceiver(
              this ScreenContainer self
            , Screen screen
            , Action<Screen, Memory<object>> onBeforePush = null
            , Action<Screen, Memory<object>> onAfterPush = null
            , Action<Screen, Memory<object>> onBeforePop = null
            , Action<Screen, Memory<object>> onAfterPop = null
        )
        {
            var callbackReceiver = new AnonymousScreenContainerCallbackReceiver();

            callbackReceiver.OnBeforePush += (enterScreen, exitScreen, args) =>
            {
                if (enterScreen.Equals(screen))
                {
                    onBeforePush?.Invoke(exitScreen, args);
                }
            };

            callbackReceiver.OnAfterPush += (enterScreen, exitScreen, args) =>
            {
                if (enterScreen.Equals(screen))
                {
                    onAfterPush?.Invoke(exitScreen, args);
                }
            };

            callbackReceiver.OnBeforePop += (enterScreen, exitScreen, args) =>
            {
                if (exitScreen.Equals(screen))
                {
                    onBeforePop?.Invoke(enterScreen, args);
                }
            };

            callbackReceiver.OnAfterPop += (enterScreen, exitScreen, args) =>
            {
                if (exitScreen.Equals(screen))
                {
                    onAfterPop?.Invoke(enterScreen, args);
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