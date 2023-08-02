using Cysharp.Threading.Tasks;
using Runtime.Definition;
using Runtime.Localization;
using System;

namespace Runtime.ConfigModel
{
    [Serializable]
    public class GetMoreCoinsWhenGoShopShopInGameDataConfigItem : ShopInGameDataConfigItem
    {
        public override ShopInGameItemType ShopInGameType => ShopInGameItemType.GetMoreCoinsWhenGoShop;
        public int numberOfCoins;
    }

    public class GetMoreCoinsWhenGoShopShopInGameDataConfig : ShopInGameDataConfig<GetMoreCoinsWhenGoShopShopInGameDataConfigItem>
    {
        protected override async UniTask<(string, string)> GetDescription(GetMoreCoinsWhenGoShopShopInGameDataConfigItem itemData)
        {
            var title = await LocalizeManager.GetLocalizeAsync(LocalizeTable.SHOP_ITEM, LocalizeKeys.GetShopItemName(itemData.ShopInGameType, itemData.dataId));
            var description = await LocalizeManager.GetLocalizeAsync(LocalizeTable.SHOP_ITEM, LocalizeKeys.GetShopItemDescription(itemData.ShopInGameType, itemData.dataId));
            return (title, description);
        }
    }
}
