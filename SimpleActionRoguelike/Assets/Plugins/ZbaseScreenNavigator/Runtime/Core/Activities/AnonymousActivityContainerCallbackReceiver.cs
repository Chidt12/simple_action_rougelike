using System;

namespace ZBase.UnityScreenNavigator.Core.Activities
{
    public class AnonymousActivityContainerCallbackReceiver : IActivityContainerCallbackReceiver
    {
        public event Action<Activity, Memory<object>> OnAfterHide;
        public event Action<Activity, Memory<object>> OnAfterShow;
        public event Action<Activity, Memory<object>> OnBeforeHide;
        public event Action<Activity, Memory<object>> OnBeforeShow;

        public AnonymousActivityContainerCallbackReceiver(
              Action<Activity, Memory<object>> onBeforeShow = null
            , Action<Activity, Memory<object>> onAfterShow = null
            , Action<Activity, Memory<object>> onBeforeHide = null
            , Action<Activity, Memory<object>> onAfterHide = null
        )
        {
            OnBeforeShow = onBeforeShow;
            OnAfterShow = onAfterShow;
            OnBeforeHide = onBeforeHide;
            OnAfterHide = onAfterHide;
        }

        void IActivityContainerCallbackReceiver.BeforeShow(Activity activity, Memory<object> args)
        {
            OnBeforeShow?.Invoke(activity, args);
        }

        void IActivityContainerCallbackReceiver.AfterShow(Activity activity, Memory<object> args)
        {
            OnAfterShow?.Invoke(activity, args);
        }

        void IActivityContainerCallbackReceiver.BeforeHide(Activity activity, Memory<object> args)
        {
            OnBeforeHide?.Invoke(activity, args);
        }

        void IActivityContainerCallbackReceiver.AfterHide(Activity activity, Memory<object> args)
        {
            OnAfterHide?.Invoke(activity, args);
        }
    }
}