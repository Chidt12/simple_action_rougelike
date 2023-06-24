using Cysharp.Threading.Tasks;
using Runtime.Definition;
using Runtime.Localization;
using System;

namespace Runtime.ConfigModel
{
    [Serializable]
    public class CritAfterHitsShopInGameDataConfigItem : ShopInGameDataConfigItem
    {
        public override ShopInGameItemType ShopInGameType => ShopInGameItemType.CritAfterHits;
        public int numberOfHit;
    }

    public class CritAfterHitsShopInGameDataConfig : ShopInGameDataConfig<CritAfterHitsShopInGameDataConfigItem>
    {
        protected override async UniTask<(string, string)> GetDescription(CritAfterHitsShopInGameDataConfigItem itemData)
        {
            var title = await LocalizeManager.GetLocalizeAsync(LocalizeTable.SHOP_ITEM, LocalizeKeys.GetShopItemName(itemData.ShopInGameType, itemData.dataId));
            var description = string.Empty;
            return (title, description);
        }
    }
}
