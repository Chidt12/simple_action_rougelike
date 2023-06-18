using System;

namespace ZBase.UnityScreenNavigator.Core.Sheets
{
    public interface ISheetContainerCallbackReceiver
    {
        void BeforeShow(Sheet enterSheet, Sheet exitSheet, Memory<object> args);

        void AfterShow(Sheet enterSheet, Sheet exitSheet, Memory<object> args);

        void BeforeHide(Sheet exitSheet, Memory<object> args);

        void AfterHide(Sheet exitSheet, Memory<object> args);
    }
}