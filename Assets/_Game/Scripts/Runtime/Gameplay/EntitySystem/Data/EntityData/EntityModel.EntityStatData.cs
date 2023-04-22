using System;
using System.Collections.Generic;

namespace Runtime.Gameplay.EntitySystem
{
    public partial class EntityModel : IEntityModifiedStatData
    {
        protected HealthEntityStat healthStat; // for quick access.
        protected Dictionary<StatType, EntityStat> statsDictionary;

        protected void InitStats()
        {
            statsDictionary = new();
            statsDictionary.Add(StatType.MoveSpeed, new EntityStat(8));
            healthStat = new HealthEntityStat(100);
            statsDictionary.Add(StatType.Health, healthStat);
        }

        public void BuffStat(StatType statType, float value, StatModifyType statModifyType)
        {
            if (!statsDictionary.ContainsKey(statType))
            {
                statsDictionary.Add(statType, new EntityStat(0));
            }

            var buffedStat = statsDictionary[statType];
            buffedStat.BuffValue(value, statModifyType);
        }

        public void DebuffStat(StatType statType, float value, StatModifyType statModifyType)
        {
            if (!statsDictionary.ContainsKey(statType))
            {
                statsDictionary.Add(statType, new EntityStat(0));
            }

            var buffedStat = statsDictionary[statType];
            buffedStat.DebuffValue(value, statModifyType);
        }

        public float GetBaseStatValue(StatType statType)
        {
            var result = statsDictionary.TryGetValue(statType, out var stat);
            return result ? stat.BaseValue : 0;
        }

        public float GetTotalStatValue(StatType statType)
        {
            var result = statsDictionary.TryGetValue(statType, out var stat);
            return result ? stat.TotalValue : 0;
        }

        public bool TryGetStat(StatType statType, out EntityStat statValue)
        {
            if (statsDictionary.TryGetValue(statType, out var stat))
            {
                statValue = stat;
                return true;
            }
            else
            {
                statValue = null;
                return false;
            }
        }

        public void GetDamage(float damage, EffectSource damageSource, EffectProperty damageProperty)
        {
            healthStat.TakeDamage(damage, damageSource, damageProperty);
        }

        public void Heal(float value, EffectSource healSource, EffectProperty healDamage)
        {
            healthStat.TakeDamage(value, healSource, healDamage);
        }
    }
}
