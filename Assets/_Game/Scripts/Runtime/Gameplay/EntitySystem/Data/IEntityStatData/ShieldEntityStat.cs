using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public class ShieldEntityStat : EntityStat
    {
        private float currentValue;

        public float CurrentValue => currentValue;

        public ShieldEntityStat(float baseValue) : base(baseValue)
        {
            currentValue = TotalValue;
        }

        public override void BuffValue(float value, StatModifyType statModifyType)
        {
            base.BuffValue(value, statModifyType);
            currentValue = TotalValue;
        }

        public override void DebuffValue(float value, StatModifyType statModifyType)
        {
            base.DebuffValue(value, statModifyType);
            currentValue = TotalValue;
        }

        public void Heal(float value)
        {
            currentValue = Mathf.Min(currentValue + value, TotalValue);
        }

        public void TakeDamage(float value)
        {
            currentValue = Mathf.Max(0, currentValue - value);
        }
    }
}
