using System;

namespace ZBase.UnityScreenNavigator.Core.Sheets
{
    public sealed class AnonymousSheetContainerCallbackReceiver : ISheetContainerCallbackReceiver
    {
        public event Action<Sheet, Sheet, Memory<object>> OnBeforeShow;
        public event Action<Sheet, Sheet, Memory<object>> OnAfterShow;
        public event Action<Sheet, Memory<object>> OnBeforeHide;
        public event Action<Sheet, Memory<object>> OnAfterHide;

        public AnonymousSheetContainerCallbackReceiver(
              Action<Sheet, Sheet, Memory<object>> onBeforeShow = null
            , Action<Sheet, Sheet, Memory<object>> onAfterShow = null
            , Action<Sheet, Memory<object>> onBeforeHide = null
            , Action<Sheet, Memory<object>> onAfterHide = null
        )
        {
            OnBeforeShow = onBeforeShow;
            OnAfterShow = onAfterShow;
            OnBeforeHide = onBeforeHide;
            OnAfterHide = onAfterHide;
        }

        void ISheetContainerCallbackReceiver.BeforeShow(Sheet enterSheet, Sheet exitSheet, Memory<object> args)
        {
            OnBeforeShow?.Invoke(enterSheet, exitSheet, args);
        }

        void ISheetContainerCallbackReceiver.AfterShow(Sheet enterSheet, Sheet exitSheet, Memory<object> args)
        {
            OnAfterShow?.Invoke(enterSheet, exitSheet, args);
        }

        void ISheetContainerCallbackReceiver.BeforeHide(Sheet exitSheet, Memory<object> args)
        {
            OnBeforeHide?.Invoke(exitSheet, args);
        }

        void ISheetContainerCallbackReceiver.AfterHide(Sheet exitSheet, Memory<object> args)
        {
            OnAfterHide?.Invoke(exitSheet, args);
        }
    }
}