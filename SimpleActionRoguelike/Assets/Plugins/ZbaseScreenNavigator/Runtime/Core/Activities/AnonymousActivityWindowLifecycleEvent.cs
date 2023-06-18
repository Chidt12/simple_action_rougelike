using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace ZBase.UnityScreenNavigator.Core.Activities
{
    public sealed class AnonymousActivityWindowLifecycleEvent : IActivityLifecycleEvent
    {
        /// <see cref="IActivityLifecycleEvent.DidShow(Memory{object})"/>
        public event Action<Memory<object>> OnDidShow;

        /// <see cref="IActivityLifecycleEvent.DidHide(Memory{object})"/>
        public event Action<Memory<object>> OnDidHide;

        public AnonymousActivityWindowLifecycleEvent(
              Func<Memory<object>, UniTask> initialize = null
            , Func<Memory<object>, UniTask> onWillShow = null, Action<Memory<object>> onDidShow = null
            , Func<Memory<object>, UniTask> onWillHide = null, Action<Memory<object>> onDidHide = null
            , Func<UniTask> onCleanup = null
        )
        {
            if (initialize != null)
                OnInitialize.Add(initialize);

            if (onWillShow != null)
                OnWillShow.Add(onWillShow);

            OnDidShow = onDidShow;

            if (onWillHide != null)
                OnWillHide.Add(onWillHide);

            OnDidHide = onDidHide;

            if (onCleanup != null)
                OnCleanup.Add(onCleanup);
        }

        /// <see cref="IActivityLifecycleEvent.Initialize(Memory{object})"/>
        public List<Func<Memory<object>, UniTask>> OnInitialize { get; } = new();

        /// <see cref="IActivityLifecycleEvent.WillShow(Memory{object})"/>
        public List<Func<Memory<object>, UniTask>> OnWillShow { get; } = new();

        /// <see cref="IActivityLifecycleEvent.WillHide(Memory{object})"/>
        public List<Func<Memory<object>, UniTask>> OnWillHide { get; } = new();

        /// <see cref="IActivityLifecycleEvent.Cleanup"/>
        public List<Func<UniTask>> OnCleanup { get; } = new();

        async UniTask IActivityLifecycleEvent.Initialize(Memory<object> args)
        {
            foreach (var onInitialize in OnInitialize)
                await onInitialize.Invoke(args);
        }

        async UniTask IActivityLifecycleEvent.WillShow(Memory<object> args)
        {
            foreach (var onWillShowEnter in OnWillShow)
                await onWillShowEnter.Invoke(args);
        }

        void IActivityLifecycleEvent.DidShow(Memory<object> args)
        {
            OnDidShow?.Invoke(args);
        }

        async UniTask IActivityLifecycleEvent.WillHide(Memory<object> args)
        {
            foreach (var onWillHideEnter in OnWillHide)
                await onWillHideEnter.Invoke(args);
        }

        void IActivityLifecycleEvent.DidHide(Memory<object> args)
        {
            OnDidHide?.Invoke(args);
        }

        async UniTask IActivityLifecycleEvent.Cleanup()
        {
            foreach (var onCleanup in OnCleanup)
                await onCleanup.Invoke();
        }
    }
}