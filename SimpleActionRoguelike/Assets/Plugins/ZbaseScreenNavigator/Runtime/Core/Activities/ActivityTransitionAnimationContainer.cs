using System;
using UnityEngine;
using ZBase.UnityScreenNavigator.Foundation;
using Object = UnityEngine.Object;

namespace ZBase.UnityScreenNavigator.Core.Activities
{
    [Serializable]
    public class ActivityTransitionAnimationContainer
    {
        [SerializeField] private TransitionAnimation _enterAnimation = new();
        [SerializeField] private TransitionAnimation _exitAnimation = new();

        public TransitionAnimation EnterAnimation => _enterAnimation;
        public TransitionAnimation ExitAnimation => _exitAnimation;

        public ITransitionAnimation GetAnimation(bool enter)
        {
            var transition = enter ? _enterAnimation : _exitAnimation;
            return transition.GetAnimation();
        }

        [Serializable]
        public class TransitionAnimation
        {
            [SerializeField] private AnimationAssetType _assetType;

            [SerializeField]
            [EnabledIf(nameof(_assetType), (int)AnimationAssetType.MonoBehaviour)]
            private TransitionAnimationBehaviour _animationBehaviour;

            [SerializeField]
            [EnabledIf(nameof(_assetType), (int)AnimationAssetType.ScriptableObject)]
            private TransitionAnimationObject _animationObject;

            public AnimationAssetType AssetType
            {
                get => _assetType;
                set => _assetType = value;
            }

            public TransitionAnimationBehaviour AnimationBehaviour
            {
                get => _animationBehaviour;
                set => _animationBehaviour = value;
            }

            public TransitionAnimationObject AnimationObject
            {
                get => _animationObject;
                set => _animationObject = value;
            }

            public ITransitionAnimation GetAnimation()
            {
                switch (_assetType)
                {
                    case AnimationAssetType.MonoBehaviour:
                        return _animationBehaviour;
                    case AnimationAssetType.ScriptableObject:
                        return Object.Instantiate(_animationObject);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}