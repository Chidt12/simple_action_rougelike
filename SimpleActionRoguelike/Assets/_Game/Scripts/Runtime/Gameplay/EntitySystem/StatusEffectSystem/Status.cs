using Runtime.Definition;
using System.Threading;

namespace Runtime.Gameplay.EntitySystem
{
    public abstract class Status<T> : IStatus where T : StatusModel
    {
        protected T ownerModel;
        protected IEntityData creator;
        protected IEntityStatusData owner;
        protected CancellationTokenSource cancellationTokenSource;

        public StatusModel OwnerModel => ownerModel;

        public abstract StatusType StatusType { get; }

        public IEntityData Creator => creator;

        public IEntityStatusData Owner => owner;

        public virtual void Dispose()
        {
            cancellationTokenSource?.Cancel();
        }

        public void Init(StatusModel statusModel, IEntityData creator, IEntityStatusData owner, StatusMetaData metaData)
        {
            ownerModel = statusModel as T;
            this.creator = creator;
            this.owner = owner;
            cancellationTokenSource = new();
            Start(metaData);
        }

        protected abstract void Start(StatusMetaData metaData);
    }
}