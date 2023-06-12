using Runtime.Definition;
using System.Collections.Generic;

namespace Runtime.Manager.Data
{
    public class EntityStatsInfo
    {
        protected Dictionary<StatType, EntityStatInfo> statsDictionary;

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

        public bool TryGetStat(StatType statType, out EntityStatInfo stat)
            => statsDictionary.TryGetValue(statType, out stat);

        public float GetStatTotalValue(StatType statType)
        {
            var result = TryGetStat(statType, out var stat);
            if (result)
                return stat.TotalValue;

            return 0;
        }

        public EntityStatsInfo()
        {
            statsDictionary = new();
        }
    }

    public class EntityStatInfo
    {
        private float _baseValue;
        private float _baseBonus;
        private float _baseMultiply;
        private float _totalMultiply;
        private float _totalBonus;

        public float BaseValue => _baseValue;
        public float TotalValue => ((_baseValue + _baseBonus) * _baseMultiply + _totalBonus) * _totalMultiply;

        public EntityStatInfo(float value)
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
