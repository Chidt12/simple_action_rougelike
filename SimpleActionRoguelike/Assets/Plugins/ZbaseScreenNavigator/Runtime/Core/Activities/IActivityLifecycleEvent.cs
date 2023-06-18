using System;
using Cysharp.Threading.Tasks;

namespace ZBase.UnityScreenNavigator.Core.Activities
{
    public interface IActivityLifecycleEvent
    {
        /// <summary>
        /// Call this method after the activity is loaded.
        /// </summary>
        /// <returns></returns>
        UniTask Initialize(Memory<object> args);

        /// <summary>
        /// Called just before this activity is displayed by the Show transition.
        /// </summary>
        /// <returns></returns>
        UniTask WillShow(Memory<object> args);

        /// <summary>
        /// Called just after this activity is displayed by the Show transition.
        /// </summary>
        void DidShow(Memory<object> args);

        /// <summary>
        /// Called just before this activity is hidden by the Hide transition.
        /// </summary>
        /// <returns></returns>
        UniTask WillHide(Memory<object> args);

        /// <summary>
        /// Called just after this activity is hidden by the Hide transition.
        /// </summary>
        void DidHide(Memory<object> args);

        /// <summary>
        /// Called just before this activity is released.
        /// </summary>
        /// <returns></returns>
        UniTask Cleanup();
    }
}