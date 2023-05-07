namespace Runtime.Constants
{
    public class Constants
    {
        public const float SCENE_LOADING_STOP_PROGRESS_VALUE = 0.9f;
    }

    public class ModalIds
    {
        public const string NONE = "NONE";
        public const string VICTORY = "prefab_modal_victory";
        public const string LOSE = "prefab_modal_lose";
    }

    public class ScreenIds
    {
        public const string START_GAME = "prefab_screen_start_game";
        public const string HOME = "prefab_screen_home";
    }

    public class AddressableKeys
    {
        #region Members

        public const string WEAPON_DATA_CONFIG_ASSET_FORMAT = "Runtime.ConfigModel.{0}WeaponDataConfig";
        public const string SKILL_DATA_CONFIG_ASSET_FORMAT = "Runtime.ConfigModel.{0}SkillDataConfig";
        public const string STATUS_DATA_CONFIG_ASSET_FORMAT = "Runtime.ConfigModel.{0}StatusDataConfig";
        public const string EQUIPMENT_MECHANIC_DATA_CONFIG_ASSET_FORMAT = "Runtime.ConfigModel.{0}EquipmentMechanicDataConfig";
        public const string SKILL_TREE_DATA_CONFIG_ASSET_FORMAT = "Runtime.ConfigModel.{0}SkillTreeDataConfig";
        public const string QUEST_DATA_CONFIG_ASSET_FORMAT = "Runtime.ConfigModel.{0}QuestDataConfig";

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