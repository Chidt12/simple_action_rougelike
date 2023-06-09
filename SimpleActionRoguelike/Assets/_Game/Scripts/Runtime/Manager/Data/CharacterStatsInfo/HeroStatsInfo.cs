using Cysharp.Threading.Tasks;
using Runtime.ConfigModel;
using Runtime.Definition;
using System.Linq;

namespace Runtime.Manager.Data
{
    public class HeroStatsInfo : EntityStatsInfo
    {
        public HeroStatsInfo(CharacterLevelStats characterLevelStats) : base(characterLevelStats)
        {
            statsDictionary.Add(StatType.AttackSpeed, new CharacterStat(0));
            statsDictionary.Add(StatType.AttackRange, new CharacterStat(0));
        }

        public UniTask UpdateBaseStatByWeapon(WeaponDataConfigItem weapon)
        {
            var stats = statsDictionary.Values.ToList();
            foreach (var stat in stats)
                stat.ResetBonusValue();

            foreach (var item in weapon.stats)
            {
                var resultCharacterStat = TryGetStat(item.statType, out var entityStat);
                if (resultCharacterStat)
                    entityStat.AddBonusValue(item.value, item.statModifyType);
            }

            return UniTask.CompletedTask;
        }
    }
}
