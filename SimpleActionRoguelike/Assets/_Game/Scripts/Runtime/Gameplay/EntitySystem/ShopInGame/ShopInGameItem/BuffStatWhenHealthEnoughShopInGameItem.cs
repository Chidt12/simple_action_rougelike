using Runtime.ConfigModel;

namespace Runtime.Gameplay.EntitySystem
{
    public class BuffStatWhenHealthEnoughShopInGameItem : ShopInGameItem<BuffStatWhenHealthEnoughShopInGameDataConfigItem>
    {
        private bool _isBuffed;

        public override void Remove()
        {
            owner.HealthStat.OnDamaged -= OnDamaged;
            owner.HealthStat.OnHealed -= OnHealed;

            if (_isBuffed)
            {
                owner.DebuffStat(dataConfigItem.buffStat.statType, dataConfigItem.buffStat.value, dataConfigItem.buffStat.statModifyType);
            }
        }

        protected override void Apply()
        {
            owner.HealthStat.OnDamaged += OnDamaged;
            owner.HealthStat.OnHealed += OnHealed;

            UpdateBuff();
        }

        private void OnHealed(float arg1, EffectSource arg2, EffectProperty arg3) => UpdateBuff();

        private void OnDamaged(float arg1, EffectSource arg2, EffectProperty arg3) => UpdateBuff();

        private void UpdateBuff()
        {
            var healthPercent = owner.HealthStat.CurrentValue / owner.HealthStat.TotalValue;
            if (dataConfigItem.isAbove)
            {
                if (healthPercent >= dataConfigItem.healthPercent)
                {
                    if (!_isBuffed)
                    {
                        _isBuffed = true;
                        owner.BuffStat(dataConfigItem.buffStat.statType, dataConfigItem.buffStat.value, dataConfigItem.buffStat.statModifyType);
                    }
                }
                else
                {
                    if (_isBuffed)
                    {
                        _isBuffed = false;
                        owner.DebuffStat(dataConfigItem.buffStat.statType, dataConfigItem.buffStat.value, dataConfigItem.buffStat.statModifyType);
                    }
                }
            }
            else
            {
                if (healthPercent <= dataConfigItem.healthPercent)
                {
                    if (!_isBuffed)
                    {
                        _isBuffed = true;
                        owner.BuffStat(dataConfigItem.buffStat.statType, dataConfigItem.buffStat.value, dataConfigItem.buffStat.statModifyType);
                    }
                }
                else
                {
                    if (_isBuffed)
                    {
                        _isBuffed = false;
                        owner.DebuffStat(dataConfigItem.buffStat.statType, dataConfigItem.buffStat.value, dataConfigItem.buffStat.statModifyType);
                    }
                }
            }
        }
    }
}