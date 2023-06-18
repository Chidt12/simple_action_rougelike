using System;

namespace ZBase.UnityScreenNavigator.Core.Sheets
{
    public static class SheetContainerExtensions
    {
        /// <summary>
        /// Add callbacks.
        /// </summary>
        /// <param name="self"></param>
        /// <param name="onBeforeShow"></param>
        /// <param name="onAfterShow"></param>
        /// <param name="onBeforeHide"></param>
        /// <param name="onAfterHide"></param>
        public static void AddCallbackReceiver(this SheetContainer self
            , Action<Sheet, Sheet, Memory<object>> onBeforeShow = null
            , Action<Sheet, Sheet, Memory<object>> onAfterShow = null
            , Action<Sheet, Memory<object>> onBeforeHide = null
            , Action<Sheet, Memory<object>> onAfterHide = null
        )
        {
            var callbackReceiver = new AnonymousSheetContainerCallbackReceiver(
                onBeforeShow, onAfterShow, onBeforeHide, onAfterHide
            );

            self.AddCallbackReceiver(callbackReceiver);
        }
    }
}