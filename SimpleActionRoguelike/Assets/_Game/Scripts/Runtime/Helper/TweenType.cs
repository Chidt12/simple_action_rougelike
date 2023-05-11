using System;
using UnityEngine;

namespace Runtime.Helper
{
    public enum TweenDefinitionTypes { MMTween, AnimationCurve }

    [Serializable]
    public class TweenType
    {
        public TweenDefinitionTypes MMTweenDefinitionType = TweenDefinitionTypes.MMTween;
        public TweenHelper.TweenCurve MMTweenCurve = TweenHelper.TweenCurve.EaseInCubic;
        public AnimationCurve Curve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1f));

        public TweenType(TweenHelper.TweenCurve newCurve)
        {
            MMTweenCurve = newCurve;
            MMTweenDefinitionType = TweenDefinitionTypes.MMTween;
        }
        public TweenType(AnimationCurve newCurve)
        {
            Curve = newCurve;
            MMTweenDefinitionType = TweenDefinitionTypes.AnimationCurve;
        }
    }
}