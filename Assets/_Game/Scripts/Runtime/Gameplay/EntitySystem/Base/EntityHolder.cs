using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public class EntityHolder : Disposable, IEntityHolder
    {
        protected IEntityData entityData;

        protected CancellationTokenSource disposeCancellationTokenSource;
        private List<IUpdateEntityBehavior> UpdateBehaviors { get; set; }
        private List<IDisposeEntityBehavior> DisposableBehaviors { get; set; }

        public IEntityData EntityData => entityData;

        public async UniTask<bool> BuildAsync(IEntityData entityData)
        {
            this.entityData = entityData;

            UpdateBehaviors = new();
            DisposableBehaviors = new();
            disposeCancellationTokenSource = new CancellationTokenSource();

            var behaviors = GetComponents<IEntityBehavior>();
            var activatedBehaviors = new List<IEntityBehavior>();
            foreach (var behavior in behaviors)
            {
                var result = await behavior.BuildAsync(this.entityData, disposeCancellationTokenSource.Token);
                if (result)
                {
                    activatedBehaviors.Add(behavior);
                    if (behavior is IUpdateEntityBehavior)
                        UpdateBehaviors.Add((IUpdateEntityBehavior)behavior);
                    if (behavior is IDisposeEntityBehavior)
                        DisposableBehaviors.Add((IDisposeEntityBehavior)behavior);
                }
            }

            StartUpdateAsync().Forget();
            return true;
        }

        private async UniTaskVoid StartUpdateAsync()
        {
            while (true)
            {
                foreach (var updateBehavior in UpdateBehaviors)
                    updateBehavior.OnUpdate(Time.deltaTime);
                await UniTask.Yield(disposeCancellationTokenSource.Token);
            }
        }

        public override void Dispose()
        {
            disposeCancellationTokenSource?.Cancel();
            foreach (var item in DisposableBehaviors)
                item.Dispose();
        }
    }
}