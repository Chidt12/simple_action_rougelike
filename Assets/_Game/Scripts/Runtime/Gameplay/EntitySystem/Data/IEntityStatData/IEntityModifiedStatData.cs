using System;

namespace Runtime.Gameplay.EntitySystem
{
    public interface IEntityModifiedStatData : IEntityStatData
    {
        void GetDamage(float damage, EffectSource damageSource, EffectProperty damageProperty);
        void Heal(float value, EffectSource damageSource, EffectProperty damageProperty);
        void BuffStat(StatType statType, float value, StatModifyType statModifyType);
        void DebuffStat(StatType statType, float value, StatModifyType statModifyType);
    }
}
