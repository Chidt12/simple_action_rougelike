using System;

namespace ZBase.UnityScreenNavigator.Core.Activities
{
    public static class ActivityContainerExtensions
    {
        public static void AddCallbackReceiver(this ActivityContainer self,
              Action<Activity, Memory<object>> onBeforeShow = null
            , Action<Activity, Memory<object>> onAfterShow = null
            , Action<Activity, Memory<object>> onBeforeHide = null
            , Action<Activity, Memory<object>> onAfterHide = null
        )
        {
            var callbackReceiver = new AnonymousActivityContainerCallbackReceiver(
                onBeforeShow, onAfterShow, onBeforeHide, onAfterHide
            );

            self.AddCallbackReceiver(callbackReceiver);
        }
        
        public static void AddCallbackReceiver(
              this ActivityContainer self
            , Activity activity
            , Action<Activity, Memory<object>> onBeforePush = null
            , Action<Activity, Memory<object>> onAfterPush = null
            , Action<Activity, Memory<object>> onBeforePop = null
            , Action<Activity, Memory<object>> onAfterPop = null
        )
        {
            var callbackReceiver = new AnonymousActivityContainerCallbackReceiver();

            callbackReceiver.OnBeforeShow += (x, args) =>
            {
                if (x.Equals(activity))
                {
                    onBeforePush?.Invoke(x, args);
                }
            };

            callbackReceiver.OnAfterShow += (x, args) =>
            {
                if (x.Equals(activity))
                {
                    onAfterPush?.Invoke(x, args);
                }
            };

            callbackReceiver.OnBeforeHide += (x, args) =>
            {
                if (x.Equals(activity))
                {
                    onBeforePop?.Invoke(x, args);
                }
            };

            callbackReceiver.OnAfterHide += (x, args) =>
            {
                if (x.Equals(activity))
                {
                    onAfterPop?.Invoke(x, args);
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