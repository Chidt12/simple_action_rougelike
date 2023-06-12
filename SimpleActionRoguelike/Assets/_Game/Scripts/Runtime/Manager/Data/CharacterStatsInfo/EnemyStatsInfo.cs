using Runtime.ConfigModel;
using Runtime.Definition;

namespace Runtime.Manager.Data
{
    public class EnemyStatsInfo : CharacterStatsInfo
    {
        public EnemyStatsInfo(CharacterLevelStats characterLevelStats) : base(characterLevelStats)
        {
            var enemyLevelStats = characterLevelStats as EnemyLevelStats;
            statsDictionary.Add(StatType.CollideDamage, new EntityStatInfo(enemyLevelStats.collideDamage));
        }
    }
}