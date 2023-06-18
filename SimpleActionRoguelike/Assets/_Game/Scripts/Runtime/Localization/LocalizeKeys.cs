using Runtime.Definition;

namespace Runtime.Localization
{
    public class LocalizeTable
    {
        public const string SHOP_ITEM = "ShopItem";
        public const string GENERAL = "General";
        public const string ARTIFACT = "Artifact";
        public const string UI = "UI";
    }

    public class LocalizeKeys
    {
        public static string GetStatName(StatType statType) => $"stat_{(int)statType}";
        public static string GetShopItemName(ShopInGameItemType shopInGameItemType, int dataId) => $"shop_item_{(int)shopInGameItemType}_{dataId}";
    }
}