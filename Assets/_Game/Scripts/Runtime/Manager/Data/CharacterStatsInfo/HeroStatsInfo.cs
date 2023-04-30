using Runtime.ConfigModel;
using Runtime.Definition;
using Runtime.Gameplay.EntitySystem;

namespace Runtime.Manager.Data
{
    public class HeroStatsInfo : CharacterStatsInfo
    {
        public HeroStatsInfo(CharacterLevelStats characterLevelStats) : base(characterLevelStats)
        {
            var heroLevelStats = characterLevelStats as HeroLevelStats;
            statsDictionary.Add(StatType.AttackSpeed, new CharacterStat(heroLevelStats.attackSpeed));
            statsDictionary.Add(StatType.DodgeChance, new CharacterStat(heroLevelStats.dodgeChance));
            statsDictionary.Add(StatType.CCReduction, new CharacterStat(heroLevelStats.ccReduction));
            statsDictionary.Add(StatType.CooldownReduction, new CharacterStat(heroLevelStats.cooldownReduction));
            statsDictionary.Add(StatType.DamageReduction, new CharacterStat(heroLevelStats.damageReduction));
        }
    }
}
