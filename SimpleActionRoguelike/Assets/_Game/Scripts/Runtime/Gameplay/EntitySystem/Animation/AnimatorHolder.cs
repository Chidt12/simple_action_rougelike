using System;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    [RequireComponent(typeof(Animator))]
    public class AnimatorHolder : MonoBehaviour
    {
        protected Action OperatedPointTriggeredCallbackAction { get; set; }
        protected Action EndActionCallbackAction { get; set; }

        public void SetEvents(Action operatedPointEvent, Action endPointEvent)
        {
            OperatedPointTriggeredCallbackAction = operatedPointEvent;
            EndActionCallbackAction = endPointEvent;
        }

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