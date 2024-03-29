using Runtime.Definition;
using System;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public class HealthEntityStat : EntityStat
    {
        private float currentValue;

        public float CurrentValue => currentValue;
        public Action<float, EffectSource, EffectProperty> OnDamaged { get; set; }
        public Action<float, EffectSource, EffectProperty> OnHealed { get; set; }

        public HealthEntityStat(float baseValue) : base(baseValue)
        {
            currentValue = TotalValue;
            OnDamaged = (_, _, _) => { };
            OnHealed = (_, _, _) => { };
        }

        public override void BuffValue(float value, StatModifyType statModifyType)
        {
            var previousValue = TotalValue;
            base.BuffValue(value, statModifyType);
            var increaseValue = TotalValue - previousValue;
            currentValue = Mathf.Min(TotalValue, currentValue + increaseValue);
            OnValueChanged?.Invoke(TotalValue);
        }

        public override void DebuffValue(float value, StatModifyType statModifyType)
        {
            base.DebuffValue(value, statModifyType);
            currentValue = Mathf.Min(currentValue, TotalValue);
            OnValueChanged?.Invoke(TotalValue);
        }

        public float Heal(float value, EffectSource healSource, EffectProperty healProperty)
        {
            var oldValue = currentValue;
            currentValue = Mathf.Min(TotalValue, currentValue + value);
            OnHealed.Invoke(currentValue - oldValue, healSource, healProperty);
            return currentValue - oldValue;
        }

        public float TakeDamage(float value, EffectSource damageSource, EffectProperty damageProperty)
        {
            var oldValue = currentValue;
            currentValue = Mathf.Max(0, currentValue - value);
            OnDamaged.Invoke(oldValue - currentValue, damageSource, damageProperty);
            return oldValue - currentValue;
        }
    }
}