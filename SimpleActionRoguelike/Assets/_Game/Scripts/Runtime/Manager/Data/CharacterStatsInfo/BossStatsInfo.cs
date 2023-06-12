using Runtime.ConfigModel;
using Runtime.Definition;

namespace Runtime.Manager.Data
{
    public class BossStatsInfo : CharacterStatsInfo
    {
        public BossStatsInfo(CharacterLevelStats characterLevelStats) : base(characterLevelStats)
        {
            var enemyLevelStats = characterLevelStats as BossLevelStats;
            statsDictionary.Add(StatType.CollideDamage, new EntityStatInfo(enemyLevelStats.collideDamage));
        }
    }
}