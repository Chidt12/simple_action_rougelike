using Cysharp.Threading.Tasks;
using Runtime.ConfigModel;
using Runtime.Constants;
using Runtime.Core.Message;
using Runtime.Core.Singleton;
using Runtime.Definition;
using Runtime.Gameplay.EntitySystem;
using Runtime.Manager.Data;
using Runtime.Message;
using System.Collections.Generic;
using System.Linq;

namespace Runtime.Gameplay
{
    public class GameplayDataManager : MonoSingleton<GameplayDataManager>
    {
        protected override void Awake()
        {
            base.Awake();
            LoadConfig().Forget();
        }

        private async UniTask LoadConfig()
        {
            await LoadStageLoadConfig();
            FinishedLoading();
        }

        private void FinishedLoading()
        {
            SimpleMessenger.Publish(new GameplayDataLoadedMessage());
        }

        private async UniTask LoadStageLoadConfig()
        {
            await ConfigDataManager.Instance.Load<StageLoadConfig>();
        }

        public async UniTask<(HeroStatsInfo, WeaponModel)> GetHeroDataAsync(int heroId)
        {
            var heroConfig = await ConfigDataManager.Instance.Load<HeroConfig>();
            var heroConfigItem = heroConfig.items.FirstOrDefault(x => x.id == heroId);
            var heroLevel = GameplayDataDispatcher.Instance.HeroLevel;
            var heroLevelConfigItem = heroConfigItem.levels.FirstOrDefault(x => x.level == heroLevel);


            var equipmentEquip = GameplayDataDispatcher.Instance.SelectedEquipments[EquipmentType.Weapon];
            var weaponType = (WeaponType)equipmentEquip.EquipmentId;
            var weaponData = await GetWeaponDataAsync(weaponType, equipmentEquip.RarityType, equipmentEquip.Level);
            var weaponModel = WeaponModelFactory.GetWeaponModel(weaponType, weaponData);
            var heroStatsInfo = await GameplayDataDispatcher.Instance.GetHeroStatsInfo(heroLevelConfigItem.CharacterLevelStats);

            return (heroStatsInfo, weaponModel);
        }

        public async UniTask<(EnemyStatsInfo, List<SkillModel>, int)> GetEnemyDataAsync(int enemyId, int level)
        {
            var zombieConfig = await ConfigDataManager.Instance.Load<EnemyConfig>(GetConfigAssetName<EnemyConfig>(enemyId.ToString()));
            var zombieConfigItem = zombieConfig.items.FirstOrDefault(x => x.id == enemyId);

            var zombieLevelConfigItem = zombieConfigItem.levels.FirstOrDefault(x => x.level == level);
            var skillIdentity = zombieLevelConfigItem.skillIdentity;
            SkillDataConfigItem skillDataConfigItem = null;
            var skillModels = new List<SkillModel>();

            if (skillIdentity.skillType != SkillType.None)
            {
                skillDataConfigItem = await ConfigDataManager.Instance.GetSkillDataConfigItem(skillIdentity.skillType, skillIdentity.skillDataId);
                var skillData = new SkillData(skillDataConfigItem);
                var skillModel = SkillModelFactory.GetSkillModel(skillIdentity.skillType, skillData);
                skillModels.Add(skillModel);
            }

            var enemyStatsInfo = new EnemyStatsInfo(zombieLevelConfigItem.CharacterLevelStats);
            return (enemyStatsInfo, skillModels, zombieLevelConfigItem.detectedPriority);
        }


        private async UniTask<WeaponData> GetWeaponDataAsync(WeaponType weaponType, RarityType weaponEquipmentRarityType, int weaponLevel)
        {
            string weaponDataConfigAssetName = string.Format(AddressableKeys.WEAPON_DATA_CONFIG_ASSET_FORMAT, weaponType);
            var weaponDataConfig = await ConfigDataManager.Instance.Load<WeaponDataConfig>(weaponDataConfigAssetName);
            var weaponDataConfigItem = weaponDataConfig.GetWeaponDataConfigItem();
            EquipmentMechanicDataConfigItem mechanicDataConfigItem = null;
            mechanicDataConfigItem = weaponDataConfig.GetEquipmentDataConfigItem(RarityType.Ultimate);
            return new WeaponData(weaponDataConfigItem, mechanicDataConfigItem);
        }

        private string GetConfigAssetName<T>(string id = "")
        {
            if (string.IsNullOrEmpty(id))
                return typeof(T).ToString();
            else
                return typeof(T).ToString() + "_" + id + ".csv";
        }
    }
}