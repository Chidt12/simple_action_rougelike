
using Runtime.ConfigModel;
using Runtime.Definition;

namespace Runtime.Manager.Data
{
    public class AssetStatsInfo : EntityStatsInfo
    {
        public AssetStatsInfo(AssetLevelConfigItem assetLevelConfigItem) : base()
        {
            statsDictionary.Add(StatType.Health, new EntityStatInfo(assetLevelConfigItem.destroyHits));
        }
    }
}
