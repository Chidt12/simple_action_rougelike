using Runtime.Definition;
using System;

namespace Runtime.Gameplay.EntitySystem
{
    public interface IEntityModifiedStatData : IEntityStatData
    {
        float GetDamage(float damage, EffectSource damageSource, EffectProperty damageProperty);
        float Heal(float value, EffectSource damageSource, EffectProperty damageProperty);
        void BuffStat(StatType statType, float value, StatModifyType statModifyType);
        void DebuffStat(StatType statType, float value, StatModifyType statModifyType);
    }
}
