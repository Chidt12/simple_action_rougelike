using Cysharp.Threading.Tasks;
using System;

namespace Runtime.Gameplay.EntitySystem
{
    public abstract class DurationStatus<T> : Status<T> where T : StatusModel
    {
        protected override void Start(StatusMetaData metaData)
        {
            StartCooldownAsync(metaData).Forget();
        }

        protected virtual async UniTaskVoid StartCooldownAsync(StatusMetaData metaData)
        {
            await StartStatus(metaData);
            await UniTask.Delay(TimeSpan.FromSeconds(ownerModel.Duration), cancellationToken: cancellationTokenSource.Token);
            await FinishStatus(metaData);

            owner.RemoveStatus(this);
        }

        protected abstract UniTask StartStatus(StatusMetaData metaData);
        protected abstract UniTask FinishStatus(StatusMetaData metaData);
    }
}
