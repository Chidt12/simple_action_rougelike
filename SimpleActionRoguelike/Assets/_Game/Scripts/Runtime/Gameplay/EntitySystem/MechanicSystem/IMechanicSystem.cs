using Cysharp.Threading.Tasks;
using System;

namespace Runtime.Gameplay.EntitySystem
{
    public interface IMechanicSystem : IDisposable
    {
        public int DataId { get; }
        public int Level { get; }
        public IEntityData EntityData { get; }
        public UniTask Init(IEntityControlData entityData);
        public UniTask ResetNewStage();
        public void ResetRevive();
    }
}
