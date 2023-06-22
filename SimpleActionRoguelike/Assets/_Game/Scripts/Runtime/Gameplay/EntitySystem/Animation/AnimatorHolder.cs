using System;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    [RequireComponent(typeof(Animator))]
    public class AnimatorHolder : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        protected Action OperatedPointTriggeredCallbackAction { get; set; }
        protected Action EndActionCallbackAction { get; set; }

        public void SetEvents(Action operatedPointEvent, Action endPointEvent)
        {
            OperatedPointTriggeredCallbackAction = operatedPointEvent;
            EndActionCallbackAction = endPointEvent;
        }

        public void Play(string animState) => _animator.Play(animState, 0, 0);

        #region Unity Animation Callback Event Methods

        public void TriggerWeaponOperatedPointActionEvent()
        {
            OperatedPointTriggeredCallbackAction?.Invoke();
            OperatedPointTriggeredCallbackAction = null;
        }

        public void TriggerWeaponEndActionEvent()
        {
            EndActionCallbackAction?.Invoke();
            EndActionCallbackAction = null;
        }

        #endregion Unity Animation Callback Event Methods
    }
}