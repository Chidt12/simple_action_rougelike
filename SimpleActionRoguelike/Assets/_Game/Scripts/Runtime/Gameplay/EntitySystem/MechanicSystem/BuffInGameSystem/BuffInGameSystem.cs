using Cysharp.Threading.Tasks;
using Runtime.ConfigModel;
using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    public interface IBuffInGameSystem : IMechanicSystem
    {
        public BuffInGameType BuffInGameType { get; }
        public void SetData(BuffInGameDataConfigItem ownerData);
    }

    public abstract class BuffInGameSystem<T> : IBuffInGameSystem where T : BuffInGameDataConfigItem
    {
        protected IEntityData ownerEntityData;
        protected T ownerData;

        public abstract BuffInGameType BuffInGameType { get; }

        public IEntityData EntityData => ownerEntityData;

        public int Level => ownerData.level;

        public virtual void Dispose()
        {}

        public virtual UniTask Init(IEntityData entityData)
        {
            this.ownerEntityData = entityData;
            return UniTask.CompletedTask;
        }

        public virtual void SetData(BuffInGameDataConfigItem ownerData)
        {
            this.ownerData = ownerData as T;
        }
    }
}
