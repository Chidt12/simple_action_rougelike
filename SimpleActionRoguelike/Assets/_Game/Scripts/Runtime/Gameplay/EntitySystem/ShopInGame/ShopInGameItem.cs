using Runtime.ConfigModel;
using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    public abstract class ShopInGameItem
    {
        public abstract ShopInGameItemType ShopInGameItemType { get; }
        public abstract void Apply(IEntityData entityData, ShopInGameDataConfigItem dataConfigItem);
        public abstract void Remove();
    }

    public abstract class ShopInGameItem<T> : ShopInGameItem where T : ShopInGameDataConfigItem
    {
        protected T dataConfigItem;
        protected IEntityData owner;

        public override void Apply(IEntityData entityData, ShopInGameDataConfigItem dataConfigItem)
        {
            this.dataConfigItem = dataConfigItem as T;
            this.owner = entityData;
            Apply();
        }

        protected abstract void Apply();
    }
}