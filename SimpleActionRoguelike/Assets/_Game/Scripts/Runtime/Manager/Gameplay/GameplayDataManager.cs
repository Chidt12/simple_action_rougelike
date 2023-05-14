using Cysharp.Threading.Tasks;
using Runtime.ConfigModel;
using Runtime.Constants;
using Runtime.Core.Message;
using Runtime.Core.Singleton;
using Runtime.Definition;
using Runtime.Gameplay.EntitySystem;
using Runtime.Manager.Data;
using Runtime.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            await LoadStatusConfig();
            FinishedLoading();
        }

        private void FinishedLoading()
        {
            SimpleMessenger.Publish(new GameplayDataLoadedMessage());
        }

        private async UniTask LoadStageLoadConfig() => await ConfigDataManager.Instance.Load<StageLoadConfig>();

        private async UniTask LoadStatusConfig()
        {
            var statusTypes = Enum.GetValues(typeof(StatusType)).Cast<StatusType>();
            foreach (var statusType in statusTypes)
            {
                if(HasConfig(statusType))
                {
                    await ConfigDataManager.Instance.Load<StatusDataConfig>(string.Format(AddressableKeys.STATUS_DATA_CONFIG_ASSET_FORMAT, statusType));
                }
            }
        }

        private bool HasConfig(StatusType statusType)
        {
            switch (statusType)
            {
                case StatusType.Stun:
                    return true;
                default:
                    return false;
            }
        }

        public async UniTask<(HeroStatsInfo, WeaponModel)> GetHeroDataAsync(int heroId)
        {
            var heroConfig = await ConfigDataManager.Instance.Load<HeroConfig>();
            var heroConfigItem = heroConfig.items.FirstOrDefault(x => x.id == heroId);
            var heroLevel = GameplayDataDispatcher.Instance.HeroLevel;
            var heroLevelConfigItem = heroConfigItem.levels.FirstOrDefault(x => x.level == heroLevel);


            var equipmentEquip = GameplayDataDispatcher.Instance.SelectedEquipments[EquipmentType.Weapon];
            var weaponType = (WeaponType)equipmentEquip.EquipmentId;
            var weaponData = await GetWeaponDataAsync(weaponType, RarityType.Common, equipmentEquip.Level);
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


        public async UniTask<WeaponData> GetWeaponDataAsync(WeaponType weaponType, RarityType weaponEquipmentRarityType, int weaponLevel)
        {
            var weaponDataConfig = await ConfigDataManager.Instance.LoadWeaponDataConfigItem(weaponType);
            var weaponMechanicConfig = await ConfigDataManager.Instance.LoadWeaponMechanicConfigItem(weaponType, weaponEquipmentRarityType);
            return new WeaponData(weaponDataConfig, weaponMechanicConfig);
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