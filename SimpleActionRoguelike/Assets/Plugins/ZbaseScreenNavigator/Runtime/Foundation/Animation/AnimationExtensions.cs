using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ZBase.UnityScreenNavigator.Foundation.Animation
{
    internal static class AnimationExtensions
    {
        public static async UniTask PlayAsync(this IAnimation self, IProgress<float> progress = null)
        {
            var player = Pool<AnimationPlayer>.Shared.Rent();
            player.Initialize(self);

            progress?.Report(0.0f);
            player.Play();

            while (player.IsFinished == false)
            {
                await UniTask.NextFrame();
                player.Update(Time.unscaledDeltaTime);
                progress?.Report(player.Time / self.Duration);
            }

            Pool<AnimationPlayer>.Shared.Return(player);
        }
    }
}