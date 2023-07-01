using Cysharp.Threading.Tasks;
using System;

namespace Runtime.Gameplay.EntitySystem
{
    public interface IMechanicSystem : IDisposable
    {
        public int Level { get; }
        public IEntityData EntityData { get; }
        public UniTask Init(IEntityData entityData);
        public void ResetNewStage();
        public void ResetRevive();
    }
}
