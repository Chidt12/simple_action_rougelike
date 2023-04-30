using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Constants
{
    public class Constants
    {
        public const float SCENE_LOADING_STOP_PROGRESS_VALUE = 0.9f;
    }


    public class ModalIds
    {
        public const string NONE = "NONE";
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
        public const string STATUS_EFFECT_DATA_CONFIG_ASSET_FORMAT = "Runtime.ConfigModel.{0}ModifierDataConfig";
        public const string EQUIPMENT_MECHANIC_DATA_CONFIG_ASSET_FORMAT = "Runtime.ConfigModel.{0}EquipmentMechanicDataConfig";
        public const string SKILL_TREE_DATA_CONFIG_ASSET_FORMAT = "Runtime.ConfigModel.{0}SkillTreeDataConfig";
        public const string QUEST_DATA_CONFIG_ASSET_FORMAT = "Runtime.ConfigModel.{0}QuestDataConfig";

        #endregion Members
    }
}