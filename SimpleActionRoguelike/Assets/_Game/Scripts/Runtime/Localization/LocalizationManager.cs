using Cysharp.Threading.Tasks;
using Runtime.Manager.Data;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Localization.Settings;

namespace Runtime.Localization
{
    public static class LocalizeManager
    {
        #region Class Methods

        public static async UniTask InitializeAsync()
        {
            await LocalizationSettings.InitializationOperation;
            InitSelectedLocale();
        }

        public static string GetLocalize(string table, string key)
        {
            var localize = LocalizationSettings.StringDatabase.GetLocalizedString(table, key);
            return localize;
        }

        public static async UniTask<string> GetLocalizeAsync(string table, string key, CancellationToken cancellationToken = default)
        {
            var localize = await LocalizationSettings.StringDatabase.GetLocalizedStringAsync(table, key).WithCancellation(cancellationToken);
            return localize;
        }

        public static void InitSelectedLocale()
        {
            var selectedLocalized = DataManager.Local.Load<PlayerBasicLocalData>().selectedLanguage;

            var isSelectedLocalized = false;
            if (!string.IsNullOrEmpty(selectedLocalized))
            {
                var settingLocale = LocalizationSettings.AvailableLocales.Locales.FirstOrDefault(x => x.LocaleName.Contains(selectedLocalized));
                if (settingLocale)
                {
                    isSelectedLocalized = true;
                    LocalizationSettings.SelectedLocale = settingLocale;
                }
            }

            if (!isSelectedLocalized)
            {
                var systemLocale = LocalizationSettings.AvailableLocales.Locales.FirstOrDefault(x => x.LocaleName.Contains(Application.systemLanguage.ToString()));
                if (systemLocale)
                {
                    LocalizationSettings.SelectedLocale = systemLocale;
                }
                else
                {
                    var locale = LocalizationSettings.AvailableLocales.Locales.FirstOrDefault(x => x.LocaleName.StartsWith(SystemLanguage.English.ToString()));
                    LocalizationSettings.SelectedLocale = locale;
                }
            }
        }

        #endregion Class Methods
    }
}