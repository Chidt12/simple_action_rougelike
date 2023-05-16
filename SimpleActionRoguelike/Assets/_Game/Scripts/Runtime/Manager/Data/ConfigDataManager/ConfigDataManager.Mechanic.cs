using Cysharp.Threading.Tasks;
using Runtime.ConfigModel;
using Runtime.Constants;
using Runtime.Definition;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Runtime.Manager.Data
{
    public partial class ConfigDataManager
    {

        public async UniTask<List<BuffInGameStageLoadConfigItem>> LoadCurrentSuitableBuffInGameItems(List<BuffInGameIdentity> currentBuffedItems)
        {
            var dataConfig = await Load<BuffInGameStageLoadConfig>();
            var suitableItems = new List<BuffInGameStageLoadConfigItem>();
            foreach (var item in dataConfig.items)
            {
                var currentBuffedItem = currentBuffedItems.FirstOrDefault(x => x.buffInGameType == item.identity.buffInGameType);
                if (currentBuffedItem.buffInGameType != BuffInGameType.None)
                {
                    var levelRequired = currentBuffedItem.level + 1;
                    if (item.identity.level == levelRequired)
                        suitableItems.Add(item);
                }
                else if(item.identity.level == 0)
                {
                    suitableItems.Add(item);
                }
            }

            return suitableItems;
        }

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