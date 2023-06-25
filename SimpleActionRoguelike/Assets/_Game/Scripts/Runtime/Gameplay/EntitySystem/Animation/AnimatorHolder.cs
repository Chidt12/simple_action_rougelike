using System;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    [RequireComponent(typeof(Animator))]
    public class AnimatorHolder : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        protected List<Action> OperatedPointTriggeredCallbackActions { get; set; }
        protected Action EndActionCallbackAction { get; set; }

        public void SetEvents(List<Action> operatedPointEvents, Action endPointEvent)
        {
            OperatedPointTriggeredCallbackActions = operatedPointEvents;
            EndActionCallbackAction = endPointEvent;
        }

        public void Play(string animState) => _animator.Play(animState, 0, 0);

        #region Unity Animation Callback Event Methods

        public void TriggerOperatedPointActionEvent(int index)
        {
            OperatedPointTriggeredCallbackActions[index]?.Invoke();
            OperatedPointTriggeredCallbackActions[index] = null;
        }

        public void TriggerEndActionEvent()
        {
            EndActionCallbackAction?.Invoke();
            EndActionCallbackAction = null;
            OperatedPointTriggeredCallbackActions.Clear();
        }

        #endregion Unity Animation Callback Event Methods
    }
}