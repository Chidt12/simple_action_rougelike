using Cysharp.Threading.Tasks;
using Runtime.Definition;
using Runtime.Localization;
using System;

namespace Runtime.ConfigModel
{
    [Serializable]
    public class BuffStatShopInGameDataConfigItem : ShopInGameDataConfigItem
    {
        public override ShopInGameItemType ShopInGameType => ShopInGameItemType.BuffStat;
        public StatType statType;
        public float statValue;
        public StatModifyType statModifyType;
    }

    public class BuffStatShopInGameDataConfig : ShopInGameDataConfig<BuffStatShopInGameDataConfigItem>
    {
        protected override async UniTask<(string, string)> GetDescription(BuffStatShopInGameDataConfigItem itemData)
        {
            var isPercentValue = itemData.statType.IsPercentValue() || itemData.statModifyType.IsPercentValue();
            var title = await LocalizeManager.GetLocalizeAsync(LocalizeTable.SHOP_ITEM, LocalizeKeys.GetShopItemName(itemData.ShopInGameType, itemData.dataId));
            var statName = await LocalizeManager.GetLocalizeAsync(LocalizeTable.GENERAL, LocalizeKeys.GetStatName(itemData.statType));
            var description = $"{statName} +{(isPercentValue ? itemData.statValue * 100 : itemData.statValue)} {(isPercentValue?"%":"")}";
            return(title, description);
        }
    }
}
