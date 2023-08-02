using Cysharp.Threading.Tasks;
using Runtime.Definition;
using Runtime.Localization;
using System;

namespace Runtime.ConfigModel
{
    [Serializable]
    public class BuffStatWhenLowerHealthShopInGameDataConfigItem : ShopInGameDataConfigItem
    {
        public override ShopInGameItemType ShopInGameType => ShopInGameItemType.BuffStatWhenLowerHealth;
        public EquipmentStat buffStat;
    }

    public class BuffStatWhenLowerHealthShopInGameDataConfig : ShopInGameDataConfig<BuffStatWhenLowerHealthShopInGameDataConfigItem>
    {
        protected override async UniTask<(string, string)> GetDescription(BuffStatWhenLowerHealthShopInGameDataConfigItem itemData)
        {
            var title = await LocalizeManager.GetLocalizeAsync(LocalizeTable.SHOP_ITEM, LocalizeKeys.GetShopItemName(itemData.ShopInGameType, itemData.dataId));
            var description = await LocalizeManager.GetLocalizeAsync(LocalizeTable.SHOP_ITEM, LocalizeKeys.GetShopItemDescription(itemData.ShopInGameType, itemData.dataId));
            return (title, description);
        }
    }

}