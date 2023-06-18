using System;

namespace ZBase.UnityScreenNavigator.Core.Screens
{
    public interface IScreenContainerCallbackReceiver
    {
        void BeforePush(Screen enterScreen, Screen exitScreen, Memory<object> args);

        void AfterPush(Screen enterScreen, Screen exitScreen, Memory<object> args);

        void BeforePop(Screen enterScreen, Screen exitScreen, Memory<object> args);

        void AfterPop(Screen enterScreen, Screen exitScreen, Memory<object> args);
    }
}