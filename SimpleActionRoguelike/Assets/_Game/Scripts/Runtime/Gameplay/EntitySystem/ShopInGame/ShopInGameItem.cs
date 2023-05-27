using Runtime.ConfigModel;
using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    public abstract class ShopInGameItem
    {
        public int BalancingPoint { get; protected set; }
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
            BalancingPoint = dataConfigItem.point;
            this.owner = entityData;
            Apply();
        }

        protected abstract void Apply();
    }
}