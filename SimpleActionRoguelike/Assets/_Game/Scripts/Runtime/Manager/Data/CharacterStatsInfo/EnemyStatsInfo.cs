using Runtime.ConfigModel;
using Runtime.Definition;

namespace Runtime.Manager.Data
{
    public class EnemyStatsInfo : EntityStatsInfo
    {
        public EnemyStatsInfo(CharacterLevelStats characterLevelStats) : base(characterLevelStats)
        {
            var enemyLevelStats = characterLevelStats as EnemyLevelStats;
            statsDictionary.Add(StatType.CollideDamage, new CharacterStat(enemyLevelStats.collideDamage));
        }
    }
}