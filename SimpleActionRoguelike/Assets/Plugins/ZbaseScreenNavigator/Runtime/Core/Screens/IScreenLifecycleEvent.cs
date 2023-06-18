using System;
using Cysharp.Threading.Tasks;

namespace ZBase.UnityScreenNavigator.Core.Screens
{
    public interface IScreenLifecycleEvent
    {
        /// <summary>
        /// Called just after this screen is loaded.
        /// </summary>
        /// <returns></returns>
        UniTask Initialize(Memory<object> args);

        /// <summary>
        /// Called just before this screen is displayed by the Push transition.
        /// </summary>
        /// <returns></returns>
        UniTask WillPushEnter(Memory<object> args);

        /// <summary>
        /// Called just after this screen is displayed by the Push transition.
        /// </summary>
        void DidPushEnter(Memory<object> args);

        /// <summary>
        /// Called just before this screen is hidden by the Push transition.
        /// </summary>
        /// <returns></returns>
        UniTask WillPushExit(Memory<object> args);

        /// <summary>
        /// Called just after this screen is hidden by the Push transition.
        /// </summary>
        void DidPushExit(Memory<object> args);

        /// <summary>
        /// Called just before this screen is displayed by the Pop transition.
        /// </summary>
        /// <returns></returns>
        UniTask WillPopEnter(Memory<object> args);

        /// <summary>
        /// Called just after this screen is displayed by the Pop transition.
        /// </summary>
        void DidPopEnter(Memory<object> args);

        /// <summary>
        /// Called just before this screen is hidden by the Pop transition.
        /// </summary>
        /// <returns></returns>
        UniTask WillPopExit(Memory<object> args);

        /// <summary>
        /// Called just after this screen is hidden by the Pop transition.
        /// </summary>
        void DidPopExit(Memory<object> args);

        /// <summary>
        /// Called just before this screen is released.
        /// </summary>
        /// <returns></returns>
        UniTask Cleanup();
    }
}