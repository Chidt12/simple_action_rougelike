using Newtonsoft.Json;
using Runtime.Definition;
using System;

namespace Runtime.Manager.Data
{
    [Serializable]
    public class PlayerBasicLocalData
    {
        [JsonProperty("0")]
        public WeaponType selectedWeapon;

        public PlayerBasicLocalData()
        {
            selectedWeapon = WeaponType.ShortGun;
        }
    }
}