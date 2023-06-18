using System;

namespace ZBase.UnityScreenNavigator.Core.Screens
{
    public sealed class AnonymousScreenContainerCallbackReceiver : IScreenContainerCallbackReceiver
    {
        public event Action<Screen, Screen, Memory<object>> OnAfterPop;
        public event Action<Screen, Screen, Memory<object>> OnAfterPush;
        public event Action<Screen, Screen, Memory<object>> OnBeforePop;
        public event Action<Screen, Screen, Memory<object>> OnBeforePush;

        public AnonymousScreenContainerCallbackReceiver(
              Action<Screen, Screen, Memory<object>> onBeforePush = null
            , Action<Screen, Screen, Memory<object>> onAfterPush = null
            , Action<Screen, Screen, Memory<object>> onBeforePop = null
            , Action<Screen, Screen, Memory<object>> onAfterPop = null
        )
        {
            OnBeforePush = onBeforePush;
            OnAfterPush = onAfterPush;
            OnBeforePop = onBeforePop;
            OnAfterPop = onAfterPop;
        }

        void IScreenContainerCallbackReceiver.BeforePush(Screen enterScreen, Screen exitScreen, Memory<object> args)
        {
            OnBeforePush?.Invoke(enterScreen, exitScreen, args);
        }

        void IScreenContainerCallbackReceiver.AfterPush(Screen enterScreen, Screen exitScreen, Memory<object> args)
        {
            OnAfterPush?.Invoke(enterScreen, exitScreen, args);
        }

        void IScreenContainerCallbackReceiver.BeforePop(Screen enterScreen, Screen exitScreen, Memory<object> args)
        {
            OnBeforePop?.Invoke(enterScreen, exitScreen, args);
        }

        void IScreenContainerCallbackReceiver.AfterPop(Screen enterScreen, Screen exitScreen, Memory<object> args)
        {
            OnAfterPop?.Invoke(enterScreen, exitScreen, args);
        }
    }
}