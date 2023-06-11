using Runtime.ConfigModel;
using Runtime.Definition;

namespace Runtime.Manager.Data
{
    public class BossStatsInfo : EntityStatsInfo
    {
        public BossStatsInfo(CharacterLevelStats characterLevelStats) : base(characterLevelStats)
        {
            var enemyLevelStats = characterLevelStats as BossLevelStats;
            statsDictionary.Add(StatType.CollideDamage, new CharacterStat(enemyLevelStats.collideDamage));
        }
    }
}