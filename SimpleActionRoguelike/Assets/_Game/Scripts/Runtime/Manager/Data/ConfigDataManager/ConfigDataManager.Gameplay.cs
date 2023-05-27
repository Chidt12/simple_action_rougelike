using Cysharp.Threading.Tasks;
using Runtime.ConfigModel;
using Runtime.Constants;
using Runtime.Definition;
using System.Linq;

namespace Runtime.Manager.Data
{
    public partial class ConfigDataManager
    {
        public async UniTask<DeathDataConfigItem> LoadDeathDataConfigItem(DeathDataIdentity identity)
        {
            var dataConfig = await Load<DeathDataConfig>(string.Format(AddressableKeys.DEATH_DATA_CONFIG_ASSET_FORMAT, identity.deathType));
            var dataConfigItem = dataConfig.Items.FirstOrDefault(x => x.dataId == identity.deathDataId);
            return dataConfigItem;
        }

        public async UniTask<SkillDataConfigItem> GetSkillDataConfigItem(SkillType skillType, int dataId)
        {
            string skillDataConfigAssetName = string.Format(AddressableKeys.SKILL_DATA_CONFIG_ASSET_FORMAT, skillType);
            var skillDataConfig = await Load<SkillDataConfig>(skillDataConfigAssetName);
            if (skillDataConfig == null)
                return null;
            var items = skillDataConfig.Items;
            var skillDataConfigItem = items.FirstOrDefault(x => x.dataId == dataId);
            return skillDataConfigItem;
        }
    }
}