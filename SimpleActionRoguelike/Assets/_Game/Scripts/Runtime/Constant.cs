namespace Runtime.Constants
{
    public class Constant
    {
        public const string HERO_ID = "1001";
        public const float SCENE_LOADING_STOP_PROGRESS_VALUE = 0.9f;
        public const float DELAY_TIME_FOR_FIRST_WAVE = 1f;
    }

    public class ModalIds
    {
        public const string NONE = "NONE";
        public const string VICTORY = "prefab_modal_victory";
        public const string LOSE = "prefab_modal_lose";

        public const string SELECT_INGAME_BUFF = "prefab_modal_select_ingame_buff";
        public const string SELECT_INGAME_SHOP = "prefab_modal_select_ingame_shop";
        public const string GIVE_INGAME_SHOP = "prefab_modal_give_ingame_shop";

        public const string QUIT_GAME = "prefab_modal_quit_game";
    }

    public class ScreenIds
    {
        public const string START_GAME = "prefab_screen_start_game";
        public const string HOME = "prefab_screen_home";
        public const string GAMEPLAY = "prefab_screen_gameplay";
        public const string LOBBY = "prefab_screen_lobby";

    }

    public class AddressableKeys
    {
        #region Members

        public const string WEAPON_DATA_CONFIG_ASSET_FORMAT = "Runtime.ConfigModel.{0}WeaponDataConfig";
        public const string SKILL_DATA_CONFIG_ASSET_FORMAT = "Runtime.ConfigModel.{0}SkillDataConfig";
        public const string BUFF_INGAME_DATA_CONFIG_ASSET_FORMAT = "Runtime.ConfigModel.{0}BuffInGameDataConfig";
        public const string SHOP_INGAME_DATA_CONFIG_ASSET_FORMAT = "Runtime.ConfigModel.{0}ShopInGameDataConfig";
        public const string DEATH_DATA_CONFIG_ASSET_FORMAT = "Runtime.ConfigModel.{0}DeathDataConfig";
                                                   
        public const string STATUS_DATA_CONFIG_ASSET_FORMAT = "Runtime.ConfigModel.{0}StatusDataConfig";
        public const string EQUIPMENT_MECHANIC_DATA_CONFIG_ASSET_FORMAT = "Runtime.ConfigModel.{0}EquipmentMechanicDataConfig";

        public const string GAME_BALANCING_CONFIG = "GameBalancingConfig";

        #endregion Members
    }

    public class Layers
    {
        #region Members

        public const int HERO_LAYER = 8;
        public const int ENEMY_LAYER = 7;
        public const int PROJECTILE_LAYER = 10;
        public const int OBSTACLE_LAYER = 6;
        public const int OBJECT_LAYER = 12;
        public const int TRAP_LAYER = 13;
        public static int HERO_LAYER_MASK = 1 << HERO_LAYER;
        public static int ENEMY_LAYER_MASK = 1 << ENEMY_LAYER;
        public static int PROJECTILE_LAYER_MASK = 1 << PROJECTILE_LAYER;
        public static int OBSTACLE_LAYER_MASK = 1 << OBSTACLE_LAYER;
        public static int OBJECT_LAYER_MASK = 1 << OBJECT_LAYER;
        public static int TRAP_LAYER_MASK = 1 << TRAP_LAYER;

        #endregion Members
    }
}