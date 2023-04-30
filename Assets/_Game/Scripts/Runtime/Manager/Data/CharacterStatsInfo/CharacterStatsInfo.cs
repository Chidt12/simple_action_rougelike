using Runtime.ConfigModel;
using Runtime.Definition;
using Runtime.Gameplay.EntitySystem;
using System.Collections.Generic;

namespace Runtime.Manager.Data
{
    public class CharacterStatsInfo
    {
        protected Dictionary<StatType, CharacterStat> statsDictionary;

        public List<StatType> StatTypes
        {
            get
            {
                var statTypes = new List<StatType>();
                foreach (var stat in statsDictionary)
                    statTypes.Add(stat.Key);
                return statTypes;
            }
        }

        public CharacterStatsInfo(CharacterLevelStats characterLevelStats)
        {
            statsDictionary = new();
            statsDictionary.Add(StatType.Health, new CharacterStat(characterLevelStats.hp));
            statsDictionary.Add(StatType.MoveSpeed, new CharacterStat(characterLevelStats.moveSpeed));
            statsDictionary.Add(StatType.DetectRange, new CharacterStat(characterLevelStats.detectRange));
            statsDictionary.Add(StatType.Armor, new CharacterStat(characterLevelStats.armor));
            statsDictionary.Add(StatType.ArmorPenetration, new CharacterStat(characterLevelStats.armorPenetration));
            statsDictionary.Add(StatType.CritChance, new CharacterStat(characterLevelStats.critChance));
            statsDictionary.Add(StatType.CritDamage, new CharacterStat(characterLevelStats.critDamage));
            statsDictionary.Add(StatType.LifeSteal, new CharacterStat(characterLevelStats.lifeSteal));
            statsDictionary.Add(StatType.AttackDamage, new CharacterStat(characterLevelStats.attackDamage));
        }

        public bool TryGetStat(StatType statType, out CharacterStat stat)
            => statsDictionary.TryGetValue(statType, out stat);

        public float GetStatTotalValue(StatType statType)
        {
            var result = TryGetStat(statType, out var stat);
            if (result)
                return stat.TotalValue;

            return 0;
        }

        public class CharacterStat
        {
            private float _baseValue;
            private float _baseBonus;
            private float _baseMultiply;
            private float _totalMultiply;
            private float _totalBonus;

            public float BaseValue => _baseValue;
            public float TotalValue => ((_baseValue + _baseBonus) * _baseMultiply + _totalBonus) * _totalMultiply;

            public CharacterStat(float value)
            {
                _baseValue = value;
                ResetBonusValue();
            }

            public void ResetBonusValue()
            {
                _baseBonus = 0;
                _baseMultiply = 1;
                _totalMultiply = 1;
                _totalBonus = 0;
            }

            public void SetValue(float value) => _baseValue = value;

            public void AddBonusValue(float value, StatModifyType statModifyType)
            {
                switch (statModifyType)
                {
                    case StatModifyType.BaseBonus:
                        _baseBonus += value;
                        break;
                    case StatModifyType.BaseMultiply:
                        _baseMultiply += value;
                        break;
                    case StatModifyType.TotalBonus:
                        _totalBonus += value;
                        break;
                    case StatModifyType.TotalMultiply:
                        _totalMultiply += value;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
