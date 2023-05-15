namespace Runtime.Manager.Data
{
    public static class DataManager
    {
        private static TransientData s_transientData = new();
        private static LocalData s_localData = new();

        public static TransientData Transient => s_transientData;
        public static LocalData Local => s_localData;
        public static ConfigDataManager Config => ConfigDataManager.Instance;
    }
}