using Runtime.Definition;
using Runtime.Manager.Data;
using System.Collections.Generic;

namespace Runtime.Gameplay.EntitySystem
{
    public partial class EntityModel : IEntityModifiedStatData
    {
        protected HealthEntityStat healthStat; // for quick access.
        protected ShieldEntityStat shieldStat;
        protected Dictionary<StatType, EntityStat> statsDictionary;

        public HealthEntityStat HealthStat => healthStat;

        public ShieldEntityStat ShieldStat => shieldStat;

        public virtual void InitStats(EntityStatsInfo entityStatsInfo)
        {
            statsDictionary = new();
            foreach (var statType in entityStatsInfo.StatTypes)
            {
                var statTotalValue = entityStatsInfo.GetStatTotalValue(statType);
                if (statType == StatType.Health)
                {
                    var newStat = new HealthEntityStat(statTotalValue);
                    statsDictionary.Add(statType, newStat);
                    healthStat = newStat;
                }
                else if (statType == StatType.Shield)
                {
                    var newStat = new ShieldEntityStat(statTotalValue);
                    statsDictionary.Add(statType, newStat);
                    shieldStat = newStat;
                }
                else
                {
                    if (statType.IsHaveCurrentValue())
                    {
                        var newStat = new EntityStatWithCurrentValue(statTotalValue);
                        statsDictionary.Add(statType, newStat);
                    }
                    else
                    {
                        var newStat = new EntityStat(statTotalValue);
                        statsDictionary.Add(statType, newStat);
                    }
                }
            }
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

        public float GetDamage(float damage, EffectSource damageSource, EffectProperty damageProperty)
        {
            var value = healthStat.TakeDamage(damage, damageSource, damageProperty);
            if (IsDead)
                DeathEvent.Invoke();
            return value;
        }

        public float Heal(float value, EffectSource healSource, EffectProperty healDamage)
        {
            return healthStat.TakeDamage(value, healSource, healDamage);
        }
    }
}
