using Cysharp.Threading.Tasks;
using Runtime.ConfigModel;
using Runtime.Constants;
using Runtime.Definition;
using System.Linq;

namespace Runtime.Manager.Data
{
    public partial class ConfigDataManager
    {
        public  StageLoadConfigItem GetStageConfigData(int stageId)
        {
            var stageLoadConfigs = GetData<StageLoadConfig>();
            return stageLoadConfigs.items.FirstOrDefault(x => x.stageId == stageId);
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