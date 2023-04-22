using Cysharp.Threading.Tasks;
using Runtime.ConfigModel;
using Runtime.Core.Singleton;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.AddressableAssets;

namespace Runtime.Gameplay
{
    public class GameplayDataManager : MonoSingleton<GameplayDataManager>
    {
        private Dictionary<uint, EnemyConfigItem> ZombieConfigDictionary { get; set; } = new();
        public StageLoadConfigItem StageLoadConfig { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            LoadConfig().Forget();
        }

        private async UniTask LoadConfig()
        {
            await LoadStageLoadConfig();
        }

        private async UniTask LoadStageLoadConfig()
        {
            var stageLoadConfigs = await LoadConfig<StageLoadConfig>();
            var stageId = GameplayDataDispatcher.Instance.StageId;
            StageLoadConfig = stageLoadConfigs.items.FirstOrDefault(x => x.stageId == stageId);
            Addressables.Release(stageLoadConfigs);
        }

        //public async UniTask<Tuple<HeroLevelModel, WeaponModel>> GetHeroDataAsync(uint heroId)
        //{
        //    var heroConfig = await LoadConfig<HeroConfig>();
        //    var heroConfigItem = heroConfig.items.FirstOrDefault(x => x.id == heroId);
        //    var heroLevel = DataDispatcher.Instance.HeroLevel;
        //    var heroLevelConfigItem = heroConfigItem.levels.FirstOrDefault(x => x.level == heroLevel);


        //    var equipmentEquip = DataDispatcher.Instance.SelectedEquipments[EquipmentType.Weapon];
        //    var weaponType = (WeaponType)equipmentEquip.EquipmentId;
        //    var weaponData = await GetWeaponDataAsync(weaponType, equipmentEquip.RarityType, equipmentEquip.Level);
        //    var weaponModel = WeaponModelFactory.GetWeaponModel(weaponType, weaponData);
        //    var heroStatsInfo = await DataDispatcher.Instance.GetHeroStatsInfo(heroLevelConfigItem.CharacterLevelStats);
        //    var heroLevelModel = new HeroLevelModel((uint)heroLevel,
        //                                            heroLevelConfigItem.detectedPriority,
        //                                            heroStatsInfo);

        //    Addressables.Release(heroConfig);
        //    return new(heroLevelModel, weaponModel);
        //}

        private async UniTask<T> LoadConfig<T>(string id = "") where T : class
        {
            if (string.IsNullOrEmpty(id))
            {
                var config = await Addressables.LoadAssetAsync<T>(typeof(T).ToString());
                return config;
            }
            else
            {
                var config = await Addressables.LoadAssetAsync<T>(typeof(T).ToString() + "_" + id + ".csv");
                return config;
            }
        }

    }
}