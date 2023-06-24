using Runtime.ConfigModel;
using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    public class BuffStatShopInGameItem : ShopInGameItem<BuffStatShopInGameDataConfigItem>
    {
        protected override void Apply()
        {
            owner.BuffStat(dataConfigItem.statType, dataConfigItem.statValue, dataConfigItem.statModifyType);
        }

        public override void Remove()
        {
            owner.DebuffStat(dataConfigItem.statType, dataConfigItem.statValue, dataConfigItem.statModifyType);
        }
    }
}
