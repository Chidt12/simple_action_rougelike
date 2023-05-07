using Cysharp.Threading.Tasks;
using Runtime.Definition;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public class StunStatus : DurationStatus<StunStatusModel>
    {
        public override StatusType StatusType => StatusType.Stun;

        protected override UniTask FinishStatus(StatusMetaData metaData)
        {
            if (owner is IEntityControlData)
                ((IEntityControlData)owner).SetMoveDirection(Vector2.zero);
            return UniTask.CompletedTask;
        }

        protected override UniTask StartStatus(StatusMetaData metaData)
        {
            return UniTask.CompletedTask;
        }
    }
}
