using Runtime.ConfigModel;
using Runtime.Gameplay.EntitySystem;

namespace Runtime.Manager.Data
{
    public class EnemyStatsInfo : CharacterStatsInfo
    {
        public EnemyStatsInfo(CharacterLevelStats characterLevelStats) : base(characterLevelStats)
        {
            var enemyLevelStats = characterLevelStats as EnemyLevelStats;
            statsDictionary.Add(StatType.CollideDamage, new CharacterStat(enemyLevelStats.collideDamage));
        }
    }
}