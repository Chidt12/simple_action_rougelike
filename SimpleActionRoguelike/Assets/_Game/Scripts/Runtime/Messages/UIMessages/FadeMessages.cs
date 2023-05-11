using Runtime.Helper;
using UnityEngine;
using ZBase.Foundation.PubSub;

namespace Runtime.Message
{
    public struct FadeInMessage : IMessage
    {
        public float duration;
        public TweenType curve;
        public bool ignoreTimeScale;
        public Vector3 worldPosition;
        public bool stopAfterFinished;

        public FadeInMessage(float duration, TweenType tweenType, bool ignoreTimeScale, Vector3 worldPosition, bool stopAfterFinished)
        {
            this.duration = duration;
            this.curve = tweenType;
            this.ignoreTimeScale = ignoreTimeScale;
            this.worldPosition = worldPosition;
            this.stopAfterFinished = stopAfterFinished;
        }
    }

    public struct FadeOutMessage : IMessage
    {
        public float duration;
        public TweenType curve;
        public bool ignoreTimeScale;
        public Vector3 worldPosition;
        public bool stopAfterFinished;

        public FadeOutMessage(float duration, TweenType tweenType, bool ignoreTimeScale, Vector3 worldPosition, bool stopAfterFinished)
        {
            this.duration = duration;
            this.curve = tweenType;
            this.ignoreTimeScale = ignoreTimeScale;
            this.worldPosition = worldPosition;
            this.stopAfterFinished = stopAfterFinished;
        }
    }

    public struct FadeStopMessage : IMessage { }

    public struct FadeMessage : IMessage
    {
        public float duration;
        public float targetAlpha;
        public TweenType curve;
        public bool ignoreTimeScale;
        public Vector3 worldPosition;

        public FadeMessage(float duration, float targetAlpha, TweenType tweenType, bool ignoreTimeScale, Vector3 worldPosition)
        {
            this.duration = duration;
            this.targetAlpha = targetAlpha;
            this.curve = tweenType;
            this.ignoreTimeScale = ignoreTimeScale;
            this.worldPosition = worldPosition;
        }
    }
}
