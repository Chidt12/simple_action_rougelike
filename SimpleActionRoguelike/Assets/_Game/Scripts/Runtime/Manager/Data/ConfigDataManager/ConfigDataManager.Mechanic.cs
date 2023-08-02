using Cysharp.Threading.Tasks;
using Runtime.ConfigModel;
using Runtime.Constants;
using Runtime.Definition;
using Runtime.Gameplay.EntitySystem;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Runtime.Manager.Data
{
    public enum ShopItemCategoryType
    {
        Power,
        Speed,
        Both,
    }

    public static class ListExtensions
    {
        public static void Shuffle<T>(this IList<T> list)
        {
            Random rng = new Random();

            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }   
    }

    public partial class ConfigDataManager
    {
        public async UniTask<List<ShopInGameStageLoadConfigItem>> LoadCurrentSuitableShopInGameItems(List<ShopInGameItem> currentShopInGames, int number, ShopItemCategoryType type)
        {
            var config = await Load<ShopInGameStageLoadConfig>();

            var suitableList = new List<ShopInGameStageLoadConfigItem>();
            foreach (var item in config.items)
            {
                if (item.isPower && type == ShopItemCategoryType.Speed)
                    continue;

                if (!item.isPower && type == ShopItemCategoryType.Power)
                    continue;

                if (item.canAppear > 0)
                {
                    var current = currentShopInGames.Count(x => x.DataId == item.identity.dataId && x.ShopInGameItemType == item.identity.shopInGameItemType);
                    if(current < item.canAppear)
                    {
                        suitableList.Add(item);
                    }
                }
                else
                {
                    suitableList.Add(item);
                }
            }

            var shuffleList = suitableList.ToList();
            shuffleList.Shuffle();
            return shuffleList.Take(number).ToList();
        }

        public async UniTask<ShopInGameDataConfigItem> LoadShopInGameDataConfigItem(ShopInGameItemType shopInGameItemType, int dataId)
        {
            var config = await LoadShopInGameDataConfig(shopInGameItemType);
            var configItem = config.GetItem(dataId);
            return configItem;
        }

        public async UniTask<ShopInGameDataConfig> LoadShopInGameDataConfig(ShopInGameItemType shopInGameItemType)
        {
            var config = await Load<ShopInGameDataConfig>(string.Format(AddressableKeys.SHOP_INGAME_DATA_CONFIG_ASSET_FORMAT, shopInGameItemType));
            return config;
        }


        public async UniTask<List<ArtifactStageLoadConfigItem>> LoadCurrentSuitableArtifactItems(List<ArtifactIdentity> currentBuffedItems, int number)
        {
            var dataConfig = await Load<ArtifactStageLoadConfig>();
            var suitableItems = new List<ArtifactStageLoadConfigItem>();
            foreach (var item in dataConfig.items)
            {
                var currentBuffedItem = currentBuffedItems.FirstOrDefault(x => x.artifactType == item.identity.artifactType);
                if (currentBuffedItem.artifactType != ArtifactType.None)
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

            var shuffleList = suitableItems.ToList();
            shuffleList.Shuffle();
            return suitableItems.Take(number).ToList();
        }

        public async UniTask<ArtifactDataConfigItem> LoadArtifactDataConfigItem(ArtifactType artifactType, int level, int dataId)
        {
            var config = await LoadArtifactDataConfig(artifactType);
            var configItem = config.GetArtifactItem(level, dataId);
            return configItem;
        }

        public async UniTask<ArtifactDataConfig> LoadArtifactDataConfig(ArtifactType artifacType)
        {
            var config = await Load<ArtifactDataConfig>(string.Format(AddressableKeys.ARTIFACT_DATA_CONFIG_ASSET_FORMAT, artifacType));
            return config;
        }
    }
}
