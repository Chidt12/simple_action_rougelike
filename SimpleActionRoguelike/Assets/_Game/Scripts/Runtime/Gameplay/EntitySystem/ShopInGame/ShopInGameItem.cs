using Runtime.ConfigModel;
using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    public abstract class ShopInGameItem
    {
        public int BalancingPoint { get; protected set; }
        public abstract int DataId { get; }
        public abstract ShopInGameItemType ShopInGameItemType { get; }
        public abstract void Apply(IEntityModifiedStatData entityData, ShopInGameDataConfigItem dataConfigItem);
        public abstract void Remove();
    }

    public abstract class ShopInGameItem<T> : ShopInGameItem where T : ShopInGameDataConfigItem
    {
        protected T dataConfigItem;
        protected IEntityModifiedStatData owner;

        public override ShopInGameItemType ShopInGameItemType => dataConfigItem.ShopInGameType;
        public override int DataId => dataConfigItem.dataId;

        public override void Apply(IEntityModifiedStatData entityData, ShopInGameDataConfigItem dataConfigItem)
        {
            this.dataConfigItem = dataConfigItem as T;
            BalancingPoint = dataConfigItem.point;
            this.owner = entityData;
            Apply();
        }

        protected abstract void Apply();
    }
}