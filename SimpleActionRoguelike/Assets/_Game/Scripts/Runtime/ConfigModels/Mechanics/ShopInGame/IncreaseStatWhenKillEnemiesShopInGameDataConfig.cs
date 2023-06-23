using Cysharp.Threading.Tasks;
using Runtime.Definition;
using Runtime.Localization;
using System;

namespace Runtime.ConfigModel
{
    [Serializable]
    public class IncreaseStatWhenKillEnemiesShopInGameDataConfigItem : ShopInGameDataConfigItem
    {
        public override ShopInGameItemType ShopInGameType => ShopInGameItemType.IncreaseStatWhenKillEnemies;
        public int numberEnemiesToIncrease;
        public StatType statType;
        public float valueEachTurn;
        public float maxValue;
    }

    public class IncreaseStatWhenKillEnemiesShopInGameDataConfig : ShopInGameDataConfig<IncreaseStatWhenKillEnemiesShopInGameDataConfigItem>
    {
        protected override async UniTask<(string, string)> GetDescription(IncreaseStatWhenKillEnemiesShopInGameDataConfigItem itemData)
        {
            var title = await LocalizeManager.GetLocalizeAsync(LocalizeTable.SHOP_ITEM, LocalizeKeys.GetShopItemName(itemData.ShopInGameType, itemData.dataId));
            var description = string.Empty;
            return (title, description);
        }
    }
}
