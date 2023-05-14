using Cysharp.Threading.Tasks;
using Runtime.ConfigModel;
using Runtime.Constants;
using Runtime.Definition;

namespace Runtime.Manager.Data
{
    public partial class ConfigDataManager
    {
        public async UniTask<BuffInGameDataConfigItem> LoadBuffInGameDataConfigItem(BuffInGameType buffInGameType, int level)
        {
            var assetName = string.Format(AddressableKeys.BUFF_INGAME_DATA_CONFIG_ASSET_FORMAT, buffInGameType);
            var config = await Load<BuffInGameDataConfig>(assetName);
            var configItem = config.GetBuffItem(level);
            return configItem;
        }

        public async UniTask<BuffInGameDataConfig> LoadBuffInGameDataConfig(BuffInGameType buffInGameType)
        {
            var config = await Load<BuffInGameDataConfig>(string.Format(AddressableKeys.BUFF_INGAME_DATA_CONFIG_ASSET_FORMAT, buffInGameType));
            return config;
        }
    }
}
