using Cysharp.Threading.Tasks;
using Runtime.ConfigModel;
using Runtime.Definition;
using System.Linq;

namespace Runtime.Manager.Data
{
    public class HeroStatsInfo : CharacterStatsInfo
    {
        public HeroStatsInfo(CharacterLevelStats characterLevelStats) : base(characterLevelStats)
        {
            statsDictionary.Add(StatType.AttackSpeed, new EntityStatInfo(0));
            statsDictionary.Add(StatType.AttackRange, new EntityStatInfo(0));
            statsDictionary.Add(StatType.DashNumber, new EntityStatInfo(0));
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
