using Cysharp.Threading.Tasks;
using Runtime.ConfigModel;
using Runtime.Constants;
using Runtime.Core.Singleton;
using Runtime.Definition;
using Runtime.Gameplay.EntitySystem;
using Runtime.Manager.Data;
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

        public async UniTask<(HeroLevelModel, WeaponModel)> GetHeroDataAsync(uint heroId)
        {
            var heroConfig = await LoadConfig<HeroConfig>();
            var heroConfigItem = heroConfig.items.FirstOrDefault(x => x.id == heroId);
            var heroLevel = GameplayDataDispatcher.Instance.HeroLevel;
            var heroLevelConfigItem = heroConfigItem.levels.FirstOrDefault(x => x.level == heroLevel);


            var equipmentEquip = GameplayDataDispatcher.Instance.SelectedEquipments[EquipmentType.Weapon];
            var weaponType = (WeaponType)equipmentEquip.EquipmentId;
            var weaponData = await GetWeaponDataAsync(weaponType, equipmentEquip.RarityType, equipmentEquip.Level);
            var weaponModel = WeaponModelFactory.GetWeaponModel(weaponType, weaponData);
            var heroStatsInfo = await GameplayDataDispatcher.Instance.GetHeroStatsInfo(heroLevelConfigItem.CharacterLevelStats);
            var heroLevelModel = new HeroLevelModel((uint)heroLevel,
                                                    heroLevelConfigItem.detectedPriority,
                                                    heroStatsInfo);

            Addressables.Release(heroConfig);
            return new(heroLevelModel, weaponModel);
        }

        public async UniTask<EnemyLevelModel> GetZombieDataAsync(uint enemyId, uint level)
        {
            if (!ZombieConfigDictionary.ContainsKey(enemyId))
            {
                var zombieConfig = await LoadConfig<EnemyConfig>(enemyId.ToString());
                var configItem = zombieConfig.items.FirstOrDefault(x => x.id == enemyId);
                ZombieConfigDictionary.TryAdd(enemyId, configItem);
                Addressables.Release(zombieConfig);
            }

            var zombieConfigItem = ZombieConfigDictionary[enemyId];
            var zombieLevelConfigItem = zombieConfigItem.levels.FirstOrDefault(x => x.level == level);
            var skillIdentity = zombieLevelConfigItem.skillIdentity;
            SkillDataConfigItem skillDataConfigItem = null;
            var skillModels = new List<SkillModel>();

            if (skillIdentity.skillType != SkillType.None)
            {
                skillDataConfigItem = await SkillDataFactory.GetSkillDataConfigItem(skillIdentity.skillType, skillIdentity.skillDataId);
                var skillData = new SkillData(skillDataConfigItem);
                var skillModel = SkillModelFactory.GetSkillModel(skillIdentity.skillType, skillData);
                skillModels.Add(skillModel);
            }

            var enemyStatsInfo = new EnemyStatsInfo(zombieLevelConfigItem.CharacterLevelStats);
            var zombieLevelModel = new EnemyLevelModel(level,
                                                        zombieLevelConfigItem.detectedPriority,
                                                        enemyStatsInfo,
                                                        skillModels);
            return new(zombieLevelModel);
        }


        private async UniTask<WeaponData> GetWeaponDataAsync(WeaponType weaponType, RarityType weaponEquipmentRarityType, int weaponLevel)
        {
            string weaponDataConfigAssetName = string.Format(AddressableKeys.WEAPON_DATA_CONFIG_ASSET_FORMAT, weaponType);
            var weaponDataConfig = await Addressables.LoadAssetAsync<IWeaponDataConfig>(weaponDataConfigAssetName);
            var weaponDataConfigItem = weaponDataConfig.GetWeaponDataConfigItem();
            EquipmentMechanicDataConfigItem mechanicDataConfigItem = null;
            mechanicDataConfigItem = weaponDataConfig.GetEquipmentDataConfigItem(RarityType.Ultimate);
            Addressables.Release(weaponDataConfig);
            return new WeaponData(weaponDataConfigItem, mechanicDataConfigItem);
        }

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