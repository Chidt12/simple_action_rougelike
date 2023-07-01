using Cysharp.Threading.Tasks;
using Runtime.ConfigModel;
using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    public interface IArtifactSystem : IMechanicSystem
    {
        public ArtifactType ArtifactType { get; }
        public void SetData(ArtifactDataConfigItem ownerData);
        public bool CanTrigger();
        public void Trigger();
    }

    public abstract class ArtifactSystem<T> : IArtifactSystem where T : ArtifactDataConfigItem
    {
        protected IEntityData ownerEntityData;
        protected T ownerData;

        public abstract ArtifactType ArtifactType { get; }

        public IEntityData EntityData => ownerEntityData;

        public int Level => ownerData.level;

        public virtual void Trigger()
        {}

        public virtual bool CanTrigger() => true;

        public virtual void Dispose()
        {}

        public virtual UniTask Init(IEntityData entityData)
        {
            this.ownerEntityData = entityData;
            return UniTask.CompletedTask;
        }

        public virtual void SetData(ArtifactDataConfigItem ownerData)
        {
            this.ownerData = ownerData as T;
        }

        public virtual void ResetNewStage()
        { }

        public virtual void ResetRevive()
        { }
    }
}
