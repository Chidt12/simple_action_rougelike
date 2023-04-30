using Animancer;
using System;
using System.Linq;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    [Serializable]
    public struct AnimancerAnimation
    {
        public bool isMainPart;
        public AnimationType animationType;
        public ClipTransition clipTransition;
        public Transform[] vfxSpawnPoints;
    }

    public interface IEntityAnimation
    {
        bool IsMainPart(AnimationType animationType);
        void Init(IEntityControlData controlData);
        void Play(AnimationType animationType);
        void SetTriggeredEvent(AnimationType animationType, Action<SetStateData> stateAction, Action<SetStateData> endAction);
        void Dispose();
        Transform[] GetVFXSpawnPoints(AnimationType animationType);
    }

    [RequireComponent(typeof(AnimancerComponent))]
    public class AnimancerEntityAnimation : MonoBehaviour, IEntityAnimation
    {
        [SerializeField] protected AnimancerComponent animancer;
        [SerializeField] protected AnimancerAnimation[] animations;
        [SerializeField] protected ClipTransition defaultState;

        protected AnimationType currentAnimationType;

        protected Action OperatedPointTriggeredCallbackAction { get; set; }
        protected Action EndActionCallbackAction { get; set; }


#if UNITY_EDITOR
        private void OnValidate()
        {
            animancer = GetComponent<AnimancerComponent>();
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
                animancer.Play(defaultState);
            else
                animancer.Play(animation.clipTransition);
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
            => OperatedPointTriggeredCallbackAction?.Invoke();

        public void TriggerWeaponEndActionEvent()
            => EndActionCallbackAction?.Invoke();

        #endregion Unity Animation Callback Event Methods
    }
}