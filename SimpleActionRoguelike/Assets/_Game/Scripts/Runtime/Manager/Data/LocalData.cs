using CodeStage.AntiCheat.Storage;
using Newtonsoft.Json;

namespace Runtime.Manager.Data
{
    public sealed class LocalData
    {
        private const string password = "asdf123@";

        public LocalData()
        {
            var settings = new ObscuredFileSettings(
                new EncryptionSettings(password),
                new DeviceLockSettings(),
                ObscuredFileLocation.PersistentData,
                true, false);

            ObscuredFilePrefs.Init(settings, true);
        }

        public T Load<T>()
        {
            var json = ObscuredFilePrefs.Get(nameof(T), string.Empty);
            return JsonConvert.DeserializeObject<T>(json);
        }

        public void Set<T>(T value)
        {
            ObscuredFilePrefs.Set(nameof(T), JsonConvert.SerializeObject(value));
            ObscuredFilePrefs.Save();
        }
    }
}