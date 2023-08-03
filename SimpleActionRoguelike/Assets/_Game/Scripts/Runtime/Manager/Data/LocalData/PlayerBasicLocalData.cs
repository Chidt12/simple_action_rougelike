using Newtonsoft.Json;
using Runtime.Constants;
using Runtime.Definition;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Manager.Data
{
    public enum TutorialType
    {
        GuideGameplay = 0,
    }

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

        [JsonProperty("5")]
        public int highestStage;

        [JsonProperty("4")]
        public Dictionary<TutorialType, bool> TutorialGuides { get; private set; }

        public PlayerBasicLocalData()
        {
            selectedWeapon = WeaponType.ShortGun;
            selectedLanguage = SystemLanguage.English.ToString();
            musicSettings = Constant.MAX_CONFIG_SOUND;
            sfxSettings = Constant.MAX_CONFIG_SOUND;

            TutorialGuides = new Dictionary<TutorialType, bool>();
        }

        public bool CheckCompletedTut(TutorialType type)
        {
            if (TutorialGuides.TryGetValue(type, out bool result))
            {
                return result;
            }

            return false;
        }

        public void SetCompleteTut(TutorialType type)
        {
            if (TutorialGuides.ContainsKey(type))
            {
                TutorialGuides[type] = true;
            }
            else
            {
                TutorialGuides.Add(type, true);
            }
        }
    }
}