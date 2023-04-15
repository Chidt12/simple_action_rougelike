using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public class HealthEntityStat : EntityStat
    {
        private float currentValue;

        public float CurrentValue => currentValue;

        public HealthEntityStat(float baseValue) : base(baseValue)
        {
        }

        public void Heal(float value)
        {
            currentValue += value;
        }

        public void TakeDamage()
        {

        }
    }
}