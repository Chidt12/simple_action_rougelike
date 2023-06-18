using Newtonsoft.Json;
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

        public PlayerBasicLocalData()
        {
            selectedWeapon = WeaponType.ShortGun;
            selectedLanguage = SystemLanguage.English.ToString();
        }
    }
}