using Cysharp.Threading.Tasks;
using Runtime.Manager.Audio;
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
        public bool rotateTowardFaceDirection;
        public string soundFx;
    }

    [RequireComponent(typeof(Animator))]
    public class AnimatorEntityAnimation : MonoBehaviour, IEntityAnimation
    {
        [SerializeField] protected Animator animator;
        [SerializeField] protected AnimatorAnimation[] animations;
        [SerializeField] protected string defaultState;
        [SerializeField] protected bool defaultRotateTowardFaceDirection;
        [SerializeField] protected SpriteRenderer changeColorSprite;

        protected AnimationType currentAnimationType;
        protected bool rotateTowardFaceDirection;

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

            // Besure that all event have to be done before

            if (OperatedPointTriggeredCallbackAction != null)
            {
                OperatedPointTriggeredCallbackAction.Invoke();
                OperatedPointTriggeredCallbackAction = null;
            }

            if (EndActionCallbackAction != null)
            {
                EndActionCallbackAction.Invoke();
                EndActionCallbackAction = null;
            }

            if (animation.animationType == AnimationType.None)
            {
                rotateTowardFaceDirection = defaultRotateTowardFaceDirection;
                animator.Play(defaultState, 0, 0);
            }
            else
            {
                rotateTowardFaceDirection = animation.rotateTowardFaceDirection;
                animator.Play(animation.stateName, 0, 0);
            }

            if (!string.IsNullOrEmpty(animation.soundFx))
            {
                AudioManager.Instance.PlaySfx(animation.soundFx).Forget();
            }
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

        public void ChangeColor(Color changedColor)
        {
            if(changeColorSprite)
                changeColorSprite.material.SetColor("_SpriteColor", changedColor);
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