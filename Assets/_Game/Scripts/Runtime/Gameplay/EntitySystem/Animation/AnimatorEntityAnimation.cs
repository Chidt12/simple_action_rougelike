using System;
using System.Linq;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    [Serializable]
    public struct AnimatorAnimation
    {
        public bool isMainPart;
        public AnimationType animationType;
        public string stateName;
        public Transform[] vfxSpawnPoints;
    }

    public interface IEntityAnimation
    {
        bool IsMainPart(AnimationType animationType);
        void Init(IEntityControlData controlData);
        void Play(AnimationType animationType);
        void Continue();
        void Pause();
        void SetTriggeredEvent(AnimationType animationType, Action<SetStateData> stateAction, Action<SetStateData> endAction);
        void Dispose();
        Transform[] GetVFXSpawnPoints(AnimationType animationType);
    }

    [RequireComponent(typeof(Animator))]
    public class AnimatorEntityAnimation : MonoBehaviour, IEntityAnimation
    {
        [SerializeField] protected Animator animator;
        [SerializeField] protected AnimatorAnimation[] animations;
        [SerializeField] protected string defaultState;

        protected AnimationType currentAnimationType;

        protected Action OperatedPointTriggeredCallbackAction { get; set; }
        protected Action EndActionCallbackAction { get; set; }


#if UNITY_EDITOR
        private void OnValidate()
        {
            animator = GetComponent<Animator>();
        }
#endif

        public virtual void Init(IEntityControlData controlData)
        { }

        public virtual bool IsMainPart(AnimationType animationType)
        {
            var animation = animations.FirstOrDefault(x => x.animationType == animationType);
            return animation.isMainPart;
        }

        public virtual void Play(AnimationType animationType)
        {
            var animation = animations.FirstOrDefault(x => x.animationType == animationType);

            currentAnimationType = animationType;
            if (animation.animationType == AnimationType.None)
                animator.Play(defaultState, 0, 0);
            else
                animator.Play(animation.stateName, 0, 0);
        }

        public virtual void Pause()
        {
            animator.enabled = false;
        }

        public virtual void Continue()
        {
            animator.enabled = true;
        }

        public virtual void Dispose() { }

        public void SetTriggeredEvent(AnimationType animationType, Action<SetStateData> stateAction, Action<SetStateData> endAction)
        {
            OperatedPointTriggeredCallbackAction = () => stateAction?.Invoke(new SetStateData(GetVFXSpawnPoints(animationType)));
            EndActionCallbackAction = () => endAction?.Invoke(new SetStateData(GetVFXSpawnPoints(animationType))); ;
        }

        public Transform[] GetVFXSpawnPoints(AnimationType animationType)
        {
            var animation = animations.FirstOrDefault(x => x.animationType == animationType);
            if (animation.animationType == AnimationType.None)
                return null;
            else
                return animation.vfxSpawnPoints;
        }

        #region Unity Animation Callback Event Methods

        public void TriggerWeaponOperatedPointActionEvent()
        {
            OperatedPointTriggeredCallbackAction?.Invoke();
        }

        public void TriggerWeaponEndActionEvent()
        {
            EndActionCallbackAction?.Invoke();
        }

        #endregion Unity Animation Callback Event Methods
    }
}