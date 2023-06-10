using Runtime.Definition;
using System;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public class EntityStatWithCurrentValue : EntityStat
    {
        protected float currentValue;
        public float CurrentValue => currentValue;

        public Action<float> OnIncreaseCurrentValue { get; set; }
        public Action<float> OnDecreaseCurrentValue { get; set; }

        public EntityStatWithCurrentValue(float baseValue) : base(baseValue)
        {
            currentValue = TotalValue;
            OnIncreaseCurrentValue = _ => { };
            OnDecreaseCurrentValue = _ => { };
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

        public void IncreaseCurrenValue(float value)
        {
            var oldValue = currentValue;
            currentValue = Mathf.Min(TotalValue, currentValue + value);
            OnIncreaseCurrentValue.Invoke(currentValue - oldValue);
        }

        public void DecreaseCurrentValue(float value)
        {
            var oldValue = currentValue; 
            currentValue = Mathf.Max(0, currentValue - value);
            OnDecreaseCurrentValue.Invoke(oldValue - currentValue);
        }
    }

}