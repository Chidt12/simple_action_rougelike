using Cysharp.Threading.Tasks;
using Runtime.Definition;
using Runtime.Localization;
using System;

namespace Runtime.ConfigModel
{
    [Serializable]
    public class HealWhenDefeatEnemyInStatusShopInGameDataConfigItem : ShopInGameDataConfigItem
    {
        public override ShopInGameItemType ShopInGameType => ShopInGameItemType.HealWhenDefeatEnemyInStatus;
        public int healAmount;
        public StatusType triggeredStatusType;
    }

    public class HealWhenDefeatEnemyInStatusShopInGameDataConfig : ShopInGameDataConfig<HealWhenDefeatEnemyInStatusShopInGameDataConfigItem>
    {
        protected override async UniTask<(string, string)> GetDescription(HealWhenDefeatEnemyInStatusShopInGameDataConfigItem itemData)
        {
            var title = await LocalizeManager.GetLocalizeAsync(LocalizeTable.SHOP_ITEM, LocalizeKeys.GetShopItemName(itemData.ShopInGameType, itemData.dataId));
            var description = string.Empty;
            return (title, description);
        }
    }
}