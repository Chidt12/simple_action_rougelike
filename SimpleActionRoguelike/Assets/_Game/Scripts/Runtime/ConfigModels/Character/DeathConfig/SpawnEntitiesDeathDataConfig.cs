using System;
using CsvReader;

namespace Runtime.ConfigModel
{
    [Serializable]
    public class SpawnEntitiesDeathDataConfigItem : DeathDataConfigItem
    {
        [CsvColumnFormat(ColumnFormat = "spawn_{0}")]
        public SpawnedEntityInfo[] spawnEntityInfo;
    }

    public class SpawnEntitiesDeathDataConfig : DeathDataConfig<SpawnEntitiesDeathDataConfigItem>
    {
    }
}
