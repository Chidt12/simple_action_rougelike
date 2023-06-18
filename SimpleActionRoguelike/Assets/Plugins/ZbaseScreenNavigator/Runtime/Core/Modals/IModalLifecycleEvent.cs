using System;
using Cysharp.Threading.Tasks;

namespace ZBase.UnityScreenNavigator.Core.Modals
{
    public interface IModalLifecycleEvent
    {
        /// <summary>
        /// Call this method after the modal is loaded.
        /// </summary>
        /// <returns></returns>
        UniTask Initialize(Memory<object> args);

        /// <summary>
        /// Called just before this modal is displayed by the Push transition.
        /// </summary>
        /// <returns></returns>
        UniTask WillPushEnter(Memory<object> args);

        /// <summary>
        /// Called just after this modal is displayed by the Push transition.
        /// </summary>
        void DidPushEnter(Memory<object> args);

        /// <summary>
        /// Called just before this modal is hidden by the Push transition.
        /// </summary>
        /// <returns></returns>
        UniTask WillPushExit(Memory<object> args);

        /// <summary>
        /// Called just after this modal is hidden by the Push transition.
        /// </summary>
        void DidPushExit(Memory<object> args);

        /// <summary>
        /// Called just before this modal is displayed by the Pop transition.
        /// </summary>
        /// <returns></returns>
        UniTask WillPopEnter(Memory<object> args);

        /// <summary>
        /// Called just after this modal is displayed by the Pop transition.
        /// </summary>
        void DidPopEnter(Memory<object> args);

        /// <summary>
        /// Called just before this modal is hidden by the Pop transition.
        /// </summary>
        /// <returns></returns>
        UniTask WillPopExit(Memory<object> args);

        /// <summary>
        /// Called just after this modal is hidden by the Pop transition.
        /// </summary>
        void DidPopExit(Memory<object> args);

        /// <summary>
        /// Called just before this modal is released.
        /// </summary>
        /// <returns></returns>
        UniTask Cleanup();
    }
}