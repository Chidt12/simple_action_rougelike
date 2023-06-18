﻿using System;
using ZBase.UnityScreenNavigator.Foundation.Animation;

namespace ZBase.UnityScreenNavigator.Tests.PlayMode.Foundation
{
    public class FakeAnimation : IAnimation
    {
        public float Progress { get; private set; }
        public float Duration { get; }

        public FakeAnimation(float duration = 1.0f)
        {
            Duration = duration;
        }
        
        public void SetTime(float time)
        {
            time = Math.Min(Duration, time);
            Progress = time / Duration;
        }
    }
}