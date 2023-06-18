using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Serialization;
using ZBase.UnityScreenNavigator.Foundation;
using Object = UnityEngine.Object;

namespace ZBase.UnityScreenNavigator.Core.Screens
{
    [Serializable]
    public sealed class ScreenTransitionAnimationContainer
    {
        [SerializeField] private List<TransitionAnimation> _pushEnterAnimations = new();
        [SerializeField] private List<TransitionAnimation> _pushExitAnimations = new();
        [SerializeField] private List<TransitionAnimation> _popEnterAnimations = new();
        [SerializeField] private List<TransitionAnimation> _popExitAnimations = new();

        public List<TransitionAnimation> PushEnterAnimations => _pushEnterAnimations;
        public List<TransitionAnimation> PushExitAnimations => _pushExitAnimations;
        public List<TransitionAnimation> PopEnterAnimations => _popEnterAnimations;
        public List<TransitionAnimation> PopExitAnimations => _popExitAnimations;

        public ITransitionAnimation GetAnimation(bool push, bool enter, string partnerTransitionIdentifier)
        {
            var anims = GetAnimations(push, enter);
            var anim = anims.FirstOrDefault(x => x.IsValid(partnerTransitionIdentifier));
            var result = anim?.GetAnimation();
            return result;
        }

        private IReadOnlyList<TransitionAnimation> GetAnimations(bool push, bool enter)
        {
            if (push)
            {
                return enter ? _pushEnterAnimations : _pushExitAnimations;
            }

            return enter ? _popEnterAnimations : _popExitAnimations;
        }

        [Serializable]
        public sealed class TransitionAnimation
        {
            [FormerlySerializedAs("_partnerPageIdentifierRegex")] [SerializeField] private string _partnerScreenIdentifierRegex;

            [SerializeField] private AnimationAssetType _assetType;

            [SerializeField] [EnabledIf(nameof(_assetType), (int)AnimationAssetType.MonoBehaviour)]
            private TransitionAnimationBehaviour _animationBehaviour;

            [SerializeField] [EnabledIf(nameof(_assetType), (int)AnimationAssetType.ScriptableObject)]
            private TransitionAnimationObject _animationObject;

            private Regex _partnerSheetIdentifierRegexCache;

            public string PartnerScreenIdentifierRegex
            {
                get => _partnerScreenIdentifierRegex;
                set => _partnerScreenIdentifierRegex = value;
            }

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

            public bool IsValid(string partnerScreenIdentifier)
            {
                if (GetAnimation() == null)
                {
                    return false;
                }

                // If the partner identifier is not registered, the animation is always valid.
                if (string.IsNullOrEmpty(_partnerScreenIdentifierRegex))
                {
                    return true;
                }
                
                if (string.IsNullOrEmpty(partnerScreenIdentifier))
                {
                    return false;
                }
                
                if (_partnerSheetIdentifierRegexCache == null)
                {
                    _partnerSheetIdentifierRegexCache = new Regex(_partnerScreenIdentifierRegex);
                }

                return _partnerSheetIdentifierRegexCache.IsMatch(partnerScreenIdentifier);
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