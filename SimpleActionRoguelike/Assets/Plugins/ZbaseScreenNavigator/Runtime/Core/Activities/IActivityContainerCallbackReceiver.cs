using System;

namespace ZBase.UnityScreenNavigator.Core.Activities
{
    public interface IActivityContainerCallbackReceiver
    {
        void BeforeShow(Activity activity, Memory<object> args);

        void AfterShow(Activity activity, Memory<object> args);

        void BeforeHide(Activity activity, Memory<object> args);

        void AfterHide(Activity activity, Memory<object> args);
    }
}