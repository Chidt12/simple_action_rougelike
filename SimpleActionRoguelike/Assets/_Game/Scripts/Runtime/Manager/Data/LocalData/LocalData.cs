using CodeStage.AntiCheat.Storage;
using Newtonsoft.Json;

namespace Runtime.Manager.Data
{
    public sealed class LocalData
    {
        private const string password = "asdf123@";
        public PlayerBasicLocalData playerBasicLocalData;

        public LocalData()
        {
            var settings = new ObscuredFileSettings(
                new EncryptionSettings(password),
                new DeviceLockSettings(),
                ObscuredFileLocation.PersistentData,
                true, false);

            ObscuredFilePrefs.Init(settings, true);

            playerBasicLocalData = Load<PlayerBasicLocalData>();
        }

        public T Load<T>() where T : new()
        {
            var json = ObscuredFilePrefs.Get(nameof(T), string.Empty);
            if (string.IsNullOrEmpty(json))
            {
                return new T();
            }
            else
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
        }

        public void Set<T>(T value)
        {
            ObscuredFilePrefs.Set(nameof(T), JsonConvert.SerializeObject(value));
            ObscuredFilePrefs.Save();
        }

        public void ClearAllData()
        {
            ObscuredFilePrefs.DeleteAll();
        }

        public void SavePlayerData()
        {
            Set(playerBasicLocalData);
        }
    }
}