using Cysharp.Threading.Tasks;
using Runtime.Core.Message;
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
        private bool _built;

        public async UniTask<bool> BuildAsync(IEntityData entityData)
        {
            _built = false;
            HasDisposed = false;
            this.entityData = entityData;
            this.entityData.EntityTransform = transform;

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

            _built = true;
            return true;
        }

        private void Update()
        {
            if (!_built)
                return;

            foreach (var updateBehavior in UpdateBehaviors)
                updateBehavior.OnUpdate(Time.deltaTime);
        }

        private void OnDisable()
        {
            Dispose();
        }

        public override void Dispose()
        {
            if (!HasDisposed)
            {
                HasDisposed = true;
                disposeCancellationTokenSource?.Cancel();
                if(DisposableBehaviors != null)
                    foreach (var item in DisposableBehaviors)
                        item.Dispose();
            }
        }
    }
}