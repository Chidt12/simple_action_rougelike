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
        void Play(AnimationType animationType);
    }

    [RequireComponent(typeof(AnimancerComponent))]
    public class AnimancerEntityAnimation : MonoBehaviour, IEntityAnimation
    {
        [SerializeField] private AnimancerComponent _animancer;
        [SerializeField] private AnimancerAnimation[] _animations;
        [SerializeField] private ClipTransition _defaultState;

#if UNITY_EDITOR
        private void OnValidate()
        {
            _animancer = GetComponent<AnimancerComponent>();
        }
#endif

        public void Play(AnimationType animationType)
        {
            var animation = _animations.FirstOrDefault(x => x.animationType == animationType);
            if (animation.animationType == AnimationType.None)
                _animancer.Play(_defaultState);
            else
                _animancer.Play(animation.clipTransition);
        }
    }
}