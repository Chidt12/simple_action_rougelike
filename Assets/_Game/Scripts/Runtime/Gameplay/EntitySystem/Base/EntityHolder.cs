using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public class EntityHolder : Disposable, IEntityHolder
    {
        protected EntityModel entityModel;

        protected CancellationTokenSource updateCancellationTokenSource;
        private List<IUpdateEntityBehavior> UpdateBehaviors { get; set; }
        private List<IDisposeEntityBehavior> DisposableBehaviors { get; set; }

        public EntityModel EntityModel => entityModel;

        public async UniTask<bool> BuildAsync(EntityModel entityModel)
        {
            this.entityModel = entityModel;

            UpdateBehaviors = new();
            DisposableBehaviors = new();

            var behaviors = GetComponents<IEntityBehavior>();
            var activatedBehaviors = new List<IEntityBehavior>();
            foreach (var behavior in behaviors)
            {
                var result = await behavior.BuildAsync(entityModel);
                if (result)
                {
                    activatedBehaviors.Add(behavior);
                    if (behavior is IUpdateEntityBehavior)
                        UpdateBehaviors.Add((IUpdateEntityBehavior)behavior);
                    if (behavior is IDisposeEntityBehavior)
                        DisposableBehaviors.Add((IDisposeEntityBehavior)behavior);
                }
            }

            updateCancellationTokenSource = new CancellationTokenSource();
            StartUpdateAsync().Forget();
            return true;
        }

        private async UniTaskVoid StartUpdateAsync()
        {
            while (true)
            {
                foreach (var updateBehavior in UpdateBehaviors)
                    updateBehavior.OnUpdate(Time.deltaTime);
                await UniTask.Yield(updateCancellationTokenSource.Token);
            }
        }

        public override void Dispose()
        {
            updateCancellationTokenSource?.Cancel();
            foreach (var item in DisposableBehaviors)
                item.Dispose();
        }
    }
}