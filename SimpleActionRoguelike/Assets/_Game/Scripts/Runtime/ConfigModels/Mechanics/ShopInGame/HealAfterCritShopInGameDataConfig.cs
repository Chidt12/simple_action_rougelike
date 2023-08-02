using Cysharp.Threading.Tasks;
using Runtime.Definition;
using Runtime.Localization;
using System;

namespace Runtime.ConfigModel
{
    [Serializable]
    public class HealAfterCritShopInGameDataConfigItem : ShopInGameDataConfigItem
    {
        public override ShopInGameItemType ShopInGameType => ShopInGameItemType.HealAfterCrit;
        public float healAmount;
    }

    public class HealAfterCritShopInGameDataConfig : ShopInGameDataConfig<HealAfterCritShopInGameDataConfigItem>
    {
        protected override async UniTask<(string, string)> GetDescription(HealAfterCritShopInGameDataConfigItem itemData)
        {
            var title = await LocalizeManager.GetLocalizeAsync(LocalizeTable.SHOP_ITEM, LocalizeKeys.GetShopItemName(itemData.ShopInGameType, itemData.dataId));
            var description = await LocalizeManager.GetLocalizeAsync(LocalizeTable.SHOP_ITEM, LocalizeKeys.GetShopItemDescription(itemData.ShopInGameType, itemData.dataId));
            return (title, description);
        }
    }
}