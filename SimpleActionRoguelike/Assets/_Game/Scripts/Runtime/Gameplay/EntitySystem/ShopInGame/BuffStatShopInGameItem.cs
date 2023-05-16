using Runtime.ConfigModel;
using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    public class BuffStatShopInGameItem : ShopInGameItem<BuffStatShopInGameDataConfigItem>
    {
        public override ShopInGameItemType ShopInGameItemType => ShopInGameItemType.BuffStat;

        protected override void Apply()
        {
            var statData = owner as IEntityModifiedStatData;
            if(statData != null)
            {
                statData.BuffStat(dataConfigItem.statType, dataConfigItem.statValue, dataConfigItem.statModifyType);
            }
        }

        public override void Remove()
        {
            var statData = owner as IEntityModifiedStatData;
            if (statData != null)
            {
                statData.DebuffStat(dataConfigItem.statType, dataConfigItem.statValue, dataConfigItem.statModifyType);
            }
        }
    }
}
