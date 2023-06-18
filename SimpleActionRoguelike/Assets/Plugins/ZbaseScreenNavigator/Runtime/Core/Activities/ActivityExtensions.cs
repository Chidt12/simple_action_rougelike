using System;
using Cysharp.Threading.Tasks;

namespace ZBase.UnityScreenNavigator.Core.Activities
{
    public static class ActivityExtensions
    {
        public static IActivityLifecycleEvent AddLifecycleEvent(
              this Activity self
            , Func<Memory<object>, UniTask> initialize = null
            , Func<Memory<object>, UniTask> onWillShow = null, Action<Memory<object>> onDidShow = null
            , Func<Memory<object>, UniTask> onWillHide = null, Action<Memory<object>> onDidHide = null
            , Func<UniTask> onCleanup = null
            , int priority = 0
        )
        {
            var lifecycleEvent = new AnonymousActivityWindowLifecycleEvent(
                initialize,
                onWillShow, onDidShow,
                onWillHide, onDidHide,
                onCleanup
            );

            self.AddLifecycleEvent(lifecycleEvent, priority);
            return lifecycleEvent;
        }
    }
}