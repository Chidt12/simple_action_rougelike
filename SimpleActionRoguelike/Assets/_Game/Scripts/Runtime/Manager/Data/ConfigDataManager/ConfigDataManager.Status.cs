using Runtime.ConfigModel;
using Runtime.Constants;
using Runtime.Definition;
using System.Linq;

namespace Runtime.Manager.Data
{
    public partial class ConfigDataManager
    {
        public StatusDataConfigItem GetStatusDataConfig(StatusType statusType, int dataId)
        {
            var statusDataConfig = GetData<StatusDataConfig>(string.Format(AddressableKeys.STATUS_DATA_CONFIG_ASSET_FORMAT, statusType));
            var dataConfig = statusDataConfig.Items.FirstOrDefault(x => x.dataId == dataId);
            return dataConfig;
        }
    }
}
