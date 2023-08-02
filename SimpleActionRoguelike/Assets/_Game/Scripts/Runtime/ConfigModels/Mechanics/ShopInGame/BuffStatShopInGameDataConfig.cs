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
            var title = await LocalizeManager.GetLocalizeAsync(LocalizeTable.SHOP_ITEM, LocalizeKeys.GetShopItemName(itemData.ShopInGameType, itemData.dataId));
            var description = await LocalizeManager.GetLocalizeAsync(LocalizeTable.SHOP_ITEM, LocalizeKeys.GetShopItemDescription(itemData.ShopInGameType, itemData.dataId));
            return (title, description);
        }
    }
}
