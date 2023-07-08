using CsvReader;
using System;

namespace Runtime.ConfigModel
{
    [Serializable]
    public class SummonSkillDataConfigItem : SkillDataConfigItem
    {
        [CsvColumnFormat(ColumnFormat = "summoned_{0}")]
        public SpawnedEntityInfo[] summonedSpawnEntitiesInfo;
        public float summonedCenterOffsetDistance;
        public bool useOwnerLevel;
    }

    public class SummonSkillDataConfig : SkillDataConfig<SummonSkillDataConfigItem>
    {
    }
}