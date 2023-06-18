using System;
using Cysharp.Threading.Tasks;

namespace ZBase.UnityScreenNavigator.Core.Modals
{
    public static class ModalExtensions
    {
        public static void AddLifecycleEvent(
              this Modal self
            , Func<Memory<object>, UniTask> initialize = null
            , Func<Memory<object>, UniTask> onWillPushEnter = null, Action<Memory<object>> onDidPushEnter = null
            , Func<Memory<object>, UniTask> onWillPushExit = null, Action<Memory<object>> onDidPushExit = null
            , Func<Memory<object>, UniTask> onWillPopEnter = null, Action<Memory<object>> onDidPopEnter = null
            , Func<Memory<object>, UniTask> onWillPopExit = null, Action<Memory<object>> onDidPopExit = null
            , Func<UniTask> onCleanup = null
            , int priority = 0
        )
        {
            var lifecycleEvent = new AnonymousModalLifecycleEvent(
                initialize,
                onWillPushEnter, onDidPushEnter,
                onWillPushExit, onDidPushExit,
                onWillPopEnter, onDidPopEnter,
                onWillPopExit, onDidPopExit,
                onCleanup
            );

            self.AddLifecycleEvent(lifecycleEvent, priority);
        }
    }
}