using Runtime.Definition;
using System;

namespace Runtime.Gameplay.EntitySystem
{
    public class EntityStat
    {
        #region Members

        private float _baseValue;
        private float _baseMultiply;
        private float _baseBonus;
        private float _totalMultiply;
        private float _totalBonus;

        #endregion Members

        #region Properties

        public float BaseValue => _baseValue;
        public float TotalMultiply => _totalMultiply;
        public float TotalValue => ((_baseValue + _baseBonus) * _baseMultiply + _totalBonus) * _totalMultiply;

        public Action<float> OnValueChanged { get; set; }

        #endregion Properties

        #region Class Methods

        public EntityStat(float baseValue)
        {
            _baseValue = baseValue;
            _baseMultiply = 1f;
            _baseBonus = 0f;
            _totalMultiply = 1f;
            _totalBonus = 0f;
        }

        public virtual void BuffValue(float value, StatModifyType statModifyType)
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

            OnValueChanged.Invoke(TotalValue);
        }

        public virtual void DebuffValue(float value, StatModifyType statModifyType)
        {
            switch (statModifyType)
            {
                case StatModifyType.BaseBonus:
                    _baseBonus = (float)Math.Round(_baseBonus - value, 2);
                    break;

                case StatModifyType.BaseMultiply:
                    _baseMultiply = (float)Math.Round(_baseMultiply - value, 2);
                    break;

                case StatModifyType.TotalBonus:
                    _totalBonus = (float)Math.Round(_totalBonus - value, 2);
                    break;

                case StatModifyType.TotalMultiply:
                    _totalMultiply = (float)Math.Round(_totalMultiply - value, 2);
                    break;
                default:
                    break;
            }

            OnValueChanged.Invoke(TotalValue);
        }

        public void Reset()
        {
            _baseBonus = 0;
            _baseMultiply = 1;
            _totalBonus = 0;
            _totalMultiply = 1;
        }

        #endregion Class Methods
    }

}