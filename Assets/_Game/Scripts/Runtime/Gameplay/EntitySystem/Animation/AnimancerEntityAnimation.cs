using Animancer;
using System;
using System.Linq;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    [Serializable]
    public struct AnimancerAnimation
    {
        public AnimationType animationType;
        public ClipTransition clipTransition;
    }

    public interface IEntityAnimation
    {
        void Init(IEntityControlData controlData);
        void Play(AnimationType animationType);
        void Dispose();
    }

    [RequireComponent(typeof(AnimancerComponent))]
    public class AnimancerEntityAnimation : MonoBehaviour, IEntityAnimation
    {
        [SerializeField] private AnimancerComponent _animancer;
        [SerializeField] private AnimancerAnimation[] _animations;
        [SerializeField] private ClipTransition _defaultState;

        protected AnimationType currentAnimationType;

#if UNITY_EDITOR
        private void OnValidate()
        {
            _animancer = GetComponent<AnimancerComponent>();
        }
#endif

        public virtual void Init(IEntityControlData controlData)
        { }

        public virtual void Play(AnimationType animationType)
        {
            var animation = _animations.FirstOrDefault(x => x.animationType == animationType);

            currentAnimationType = animationType;

            if (animation.animationType == AnimationType.None)
                _animancer.Play(_defaultState);
            else
                _animancer.Play(animation.clipTransition);
        }

        public virtual void Dispose() { }
    }
}