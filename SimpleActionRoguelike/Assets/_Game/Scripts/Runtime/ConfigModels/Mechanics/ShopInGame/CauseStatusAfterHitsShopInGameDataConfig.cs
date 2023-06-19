using Cysharp.Threading.Tasks;
using Runtime.Definition;
using Runtime.Localization;

namespace Runtime.ConfigModel
{
    public class CauseStatusAfterHitsShopInGameDataConfigItem : ShopInGameDataConfigItem
    {
        public override ShopInGameItemType ShopInGameType => ShopInGameItemType.CauseStatusAfterHits;

        public StatusIdentity statusIdentity;
        public int numberHitTriggered;
    }

    public class CauseStatusAfterHitsShopInGameDataConfig : ShopInGameDataConfig<CauseStatusAfterHitsShopInGameDataConfigItem>
    {
        protected async override UniTask<(string, string)> GetDescription(CauseStatusAfterHitsShopInGameDataConfigItem itemData)
        {
            var title = await LocalizeManager.GetLocalizeAsync(LocalizeTable.SHOP_ITEM, LocalizeKeys.GetShopItemName(itemData.ShopInGameType, itemData.dataId));
            var description = string.Empty;
            return (title, description);
        }
    }
}