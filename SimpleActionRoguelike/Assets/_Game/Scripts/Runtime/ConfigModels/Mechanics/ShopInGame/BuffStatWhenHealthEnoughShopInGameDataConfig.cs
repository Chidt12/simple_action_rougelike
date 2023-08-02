using Cysharp.Threading.Tasks;
using Runtime.Definition;
using Runtime.Localization;
using System;

namespace Runtime.ConfigModel
{
    [Serializable]
    public class BuffStatWhenHealthEnoughShopInGameDataConfigItem : ShopInGameDataConfigItem
    {
        public override ShopInGameItemType ShopInGameType => ShopInGameItemType.BuffStatWhenHealthEnough;
        public bool isAbove;
        public float healthPercent;
        public EquipmentStat buffStat;
    }

    public class BuffStatWhenHealthEnoughShopInGameDataConfig : ShopInGameDataConfig<BuffStatWhenHealthEnoughShopInGameDataConfigItem>
    {
        protected override async UniTask<(string, string)> GetDescription(BuffStatWhenHealthEnoughShopInGameDataConfigItem itemData)
        {
            var title = await LocalizeManager.GetLocalizeAsync(LocalizeTable.SHOP_ITEM, LocalizeKeys.GetShopItemName(itemData.ShopInGameType, itemData.dataId));
            var description = await LocalizeManager.GetLocalizeAsync(LocalizeTable.SHOP_ITEM, LocalizeKeys.GetShopItemDescription(itemData.ShopInGameType, itemData.dataId));
            return (title, description);
        }
    }
}
