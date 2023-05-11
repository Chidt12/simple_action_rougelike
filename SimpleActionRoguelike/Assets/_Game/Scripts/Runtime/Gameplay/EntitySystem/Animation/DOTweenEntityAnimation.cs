using DG.Tweening;
using System;
using System.Linq;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    [Serializable]
    public struct DOTWeenAnimationItem
    {
        public bool isMainPart;
        public AnimationType animationType;
        public DOTweenAnimation dotweenAnimation;
        public Transform[] vfxSpawnPoints;
    }

    public class DOTweenEntityAnimation : MonoBehaviour, IEntityAnimation
    {
        [SerializeField] protected DOTWeenAnimationItem[] animations;

        protected AnimationType currentAnimationType;
        protected Action OperatedPointTriggeredCallbackAction { get; set; }
        protected Action EndActionCallbackAction { get; set; }

        public void ChangeColor(Color changedColor)
        {}

        public void Continue()
        {}

        public void Dispose()
        {}

        public Transform[] GetVFXSpawnPoints(AnimationType animationType)
        {
            var animation = animations.FirstOrDefault(x => x.animationType == animationType);
            if (animation.animationType == AnimationType.None)
                return null;
            else
                return animation.vfxSpawnPoints;
        }

        public void Init(IEntityControlData controlData)
        {}

        public bool IsMainPart(AnimationType animationType)
        {
            var animation = animations.FirstOrDefault(x => x.animationType == animationType);
            return animation.isMainPart;
        }

        public void Pause(){}

        public void Play(AnimationType animationType)
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

            if (animation.animationType != AnimationType.None)
                animation.dotweenAnimation.DOPlay();
        }

        public void SetTriggeredEvent(AnimationType animationType, Action<SetStateData> stateAction, Action<SetStateData> endAction)
        {
            OperatedPointTriggeredCallbackAction = () => stateAction?.Invoke(new SetStateData(GetVFXSpawnPoints(animationType)));
            EndActionCallbackAction = () => endAction?.Invoke(new SetStateData(GetVFXSpawnPoints(animationType)));
        }
    }
}