using Cysharp.Threading.Tasks;
using Runtime.Core.Message;
using Runtime.Message;
using System.Threading;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public class EntityPresentIntroduceBehavior : EntityBehavior
    {
        [SerializeField] private float _introduceTime;

        public override UniTask<bool> BuildAsync(IEntityData data, CancellationToken cancellationToken)
        {
            SimpleMessenger.Publish(new EntityIntroducedMessage(data.EntityId, data.EntityType, _introduceTime));
            return base.BuildAsync(data, cancellationToken);
        }
    }
}