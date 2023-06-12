using Runtime.ConfigModel;
using Runtime.Definition;

namespace Runtime.Manager.Data
{
    public class CharacterStatsInfo : EntityStatsInfo
    {
        public CharacterStatsInfo(CharacterLevelStats characterLevelStats) : base()
        {
            statsDictionary.Add(StatType.Health, new EntityStatInfo(characterLevelStats.hp));
            statsDictionary.Add(StatType.MoveSpeed, new EntityStatInfo(characterLevelStats.moveSpeed));
            statsDictionary.Add(StatType.DetectRange, new EntityStatInfo(characterLevelStats.detectRange));
            statsDictionary.Add(StatType.Armor, new EntityStatInfo(characterLevelStats.armor));
            statsDictionary.Add(StatType.ArmorPenetration, new EntityStatInfo(characterLevelStats.armorPenetration));
            statsDictionary.Add(StatType.CritChance, new EntityStatInfo(characterLevelStats.critChance));
            statsDictionary.Add(StatType.CritDamage, new EntityStatInfo(characterLevelStats.critDamage));
            statsDictionary.Add(StatType.LifeSteal, new EntityStatInfo(characterLevelStats.lifeSteal));
            statsDictionary.Add(StatType.AttackDamage, new EntityStatInfo(characterLevelStats.attackDamage));
        }
    }
}
