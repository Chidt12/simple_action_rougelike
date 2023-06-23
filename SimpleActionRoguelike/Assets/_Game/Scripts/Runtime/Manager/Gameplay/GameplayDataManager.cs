using Cysharp.Threading.Tasks;
using Runtime.ConfigModel;
using Runtime.Constants;
using Runtime.Core.Message;
using Runtime.Core.Singleton;
using Runtime.Definition;
using Runtime.Gameplay;
using Runtime.Gameplay.Balancing;
using Runtime.Gameplay.EntitySystem;
using Runtime.Manager.Data;
using Runtime.Message;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Runtime.Manager.Gameplay
{
    public class GameplayDataManager : MonoSingleton<GameplayDataManager>
    {
        public async UniTask InitAsync()
        {
            await LoadConfig();
        }

        public void Dispose()
        {
            DataManager.Transient.ClearInGameMoney();
        }

        public GameBalancingConfig GetGameBalancingConfig() => ConfigDataManager.Instance.GetData<GameBalancingConfig>(AddressableKeys.GAME_BALANCING_CONFIG);

        private async UniTask LoadConfig()
        {
            await LoadStatusConfig();
            await LoadGameBalancingConfig();
            FinishedLoading();
        }

        private void FinishedLoading()
        {
            SimpleMessenger.Publish(new GameplayDataLoadedMessage());
        }

        private async UniTask LoadStatusConfig()
        {
            var statusTypes = Enum.GetValues(typeof(StatusType)).Cast<StatusType>();
            foreach (var statusType in statusTypes)
            {
                if (HasConfig(statusType))
                {
                    await DataManager.Config.Load<StatusDataConfig>(string.Format(AddressableKeys.STATUS_DATA_CONFIG_ASSET_FORMAT, statusType));
                }
            }
        }

        private async UniTask LoadGameBalancingConfig()
        {
            await DataManager.Config.Load<GameBalancingConfig>(AddressableKeys.GAME_BALANCING_CONFIG);
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
            var heroConfig = await DataManager.Config.Load<HeroConfig>();
            var heroConfigItem = heroConfig.items.FirstOrDefault(x => x.id == heroId);
            var heroLevel = GameplayDataDispatcher.Instance.HeroLevel;
            var heroLevelConfigItem = heroConfigItem.levels.FirstOrDefault(x => x.level == heroLevel);

            var equipmentEquip = GameplayDataDispatcher.Instance.SelectedEquipments[EquipmentType.Weapon];
            var weaponType = (WeaponType)equipmentEquip.EquipmentId;
            var weaponData = await GetWeaponDataAsync(weaponType, RarityType.Common, equipmentEquip.Level);
            var weaponModel = WeaponModelFactory.GetWeaponModel(weaponType, weaponData);
            var heroStatsInfo = await GameplayDataDispatcher.Instance.GetHeroStatsInfo(heroLevelConfigItem.CharacterLevelStats);
            await heroStatsInfo.UpdateBaseStatByWeapon(weaponData.weaponConfigItem);
            return (heroStatsInfo, weaponModel);
        }

        public async UniTask<(EnemyStatsInfo, List<SkillModel>, EnemyLevelConfigItem)> GetEnemyDataAsync(int enemyId, int level)
        {
            var zombieConfig = await DataManager.Config.Load<EnemyConfig>(GetConfigAssetName<EnemyConfig>(enemyId.ToString()));
            var zombieConfigItem = zombieConfig.items.FirstOrDefault(x => x.id == enemyId);

            var zombieLevelConfigItem = zombieConfigItem.levels.FirstOrDefault(x => x.level == level);
            var skillIdentity = zombieLevelConfigItem.skillIdentity;
            SkillDataConfigItem skillDataConfigItem = null;
            var skillModels = new List<SkillModel>();

            if (skillIdentity.skillType != SkillType.None)
            {
                skillDataConfigItem = await DataManager.Config.GetSkillDataConfigItem(skillIdentity.skillType, skillIdentity.skillDataId);
                var skillData = new SkillData(skillDataConfigItem, skillIdentity.skillAnimIndex);
                var skillModel = SkillModelFactory.GetSkillModel(skillIdentity.skillType, skillData);
                skillModels.Add(skillModel);
            }

            var enemyStatsInfo = new EnemyStatsInfo(zombieLevelConfigItem.CharacterLevelStats);
            return (enemyStatsInfo, skillModels, zombieLevelConfigItem);
        }

        public async UniTask<(BossStatsInfo, List<SkillModel>, BossLevelConfigItem)> GetBossDataAsync(int bossId, int level)
        {
            var bossConfig = await DataManager.Config.Load<BossConfig>(GetConfigAssetName<BossConfig>(bossId.ToString()));
            var bossConfigItem = bossConfig.items.FirstOrDefault(x => x.id == bossId);

            var bossLevelConfigItem = bossConfigItem.levels.FirstOrDefault(x => x.level == level);
            var skillIdentities = bossLevelConfigItem.skillIdentities;

            var skillModels = new List<SkillModel>();
            foreach (var skillIdentity in skillIdentities)
            {
                var skillDataConfigItem = await DataManager.Config.GetSkillDataConfigItem(skillIdentity.skillType, skillIdentity.skillDataId);
                var skillData = new SkillData(skillDataConfigItem, skillIdentity.skillAnimIndex);
                var skillModel = SkillModelFactory.GetSkillModel(skillIdentity.skillType, skillData);
                skillModels.Add(skillModel);
            }

            var bossStatsInfo = new BossStatsInfo(bossLevelConfigItem.CharacterLevelStats);
            return (bossStatsInfo, skillModels, bossLevelConfigItem);
        }

        public async UniTask<(AssetStatsInfo, AssetLevelConfigItem)> GetAssetDataAsync(int assetId, int level)
        {
            var assetConfig = await DataManager.Config.Load<AssetConfig>();
            var assetConfigItem = assetConfig.items.FirstOrDefault(x => x.id == assetId);

            var assetLevelConfigItem = assetConfigItem.levels.FirstOrDefault(x => x.level == level);
            var assetStatsInfo = new AssetStatsInfo(assetLevelConfigItem);
            return (assetStatsInfo, assetLevelConfigItem);
        }

        public async UniTask<WeaponData> GetWeaponDataAsync(WeaponType weaponType, RarityType weaponEquipmentRarityType, int weaponLevel)
        {
            var weaponDataConfig = await DataManager.Config.LoadWeaponDataConfigItem(weaponType);
            var weaponMechanicConfig = await DataManager.Config.LoadWeaponMechanicConfigItem(weaponType, weaponEquipmentRarityType);
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