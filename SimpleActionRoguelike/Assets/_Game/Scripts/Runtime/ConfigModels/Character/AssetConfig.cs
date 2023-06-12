using System;

namespace Runtime.ConfigModel
{
    [Serializable]
    public struct AssetLevelConfigItem
    {
        public int level; 
        public int destroyHits;
    }

    [Serializable]
    public struct AssetConfigItem
    {
        public int id;
        public AssetLevelConfigItem[] levels;
    }

    public class AssetConfig : BaseConfig<AssetConfigItem>
    {
    }
}