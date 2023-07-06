using Newtonsoft.Json;
using Runtime.Constants;
using Runtime.Definition;
using System;
using UnityEngine;

namespace Runtime.Manager.Data
{
    [Serializable]
    public class PlayerBasicLocalData
    {
        [JsonProperty("0")]
        public WeaponType selectedWeapon;
        [JsonProperty("1")]
        public string selectedLanguage;
        [JsonProperty("2")]
        public int musicSettings;
        [JsonProperty("3")]
        public int sfxSettings;

        public PlayerBasicLocalData()
        {
            selectedWeapon = WeaponType.ShortGun;
            selectedLanguage = SystemLanguage.English.ToString();
            musicSettings = Constant.MAX_CONFIG_SOUND;
            sfxSettings = Constant.MAX_CONFIG_SOUND;
        }
    }
}