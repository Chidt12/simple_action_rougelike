using Runtime.Definition;

namespace Runtime.Localization
{
    public class LocalizeTable
    {
        public const string SHOP_ITEM = "ShopItem";
        public const string GENERAL = "General";
        public const string ARTIFACT = "Artifact";
        public const string UI = "UI";
        public const string ENTITY = "Entity";
    }

    public class LocalizeKeys
    {
        #region UI

        public static string POPUP_CONFIRM_QUIT_GAME = "popup_confirm_quit_game";
        public static string POPUP_CONFIRM_BACK_HOME = "popup_confirm_back_home";
        public static string POPUP_CONFIRM_REPLAY = "popup_confirm_replay";

        #endregion UI

        public static string GetStatName(StatType statType) => $"stat_{(int)statType}";
        public static string GetShopItemName(ShopInGameItemType shopInGameItemType, int dataId) => $"shop_item_{(int)shopInGameItemType}_{dataId}_name";
        public static string GetArtifactName(ArtifactType artifactType) => $"artifact_item_{(int)artifactType}_name";
        public static string GetEntityName(int entityId) => $"{entityId}_name";
    }
}