using Runtime.ConfigModel;

namespace Runtime.Gameplay.EntitySystem
{
    public class BuffStatWhenLowerHealthShopInGameItem : ShopInGameItem<BuffStatWhenLowerHealthShopInGameDataConfigItem>
    {
        private float _currentBuffValue;

        public override void Remove()
        {
            owner.HealthStat.OnDamaged -= OnDamaged;
            owner.HealthStat.OnHealed -= OnHealed;
            owner.DebuffStat(dataConfigItem.buffStat.statType, _currentBuffValue, dataConfigItem.buffStat.statModifyType);
        }

        protected override void Apply()
        {
            _currentBuffValue = 0;
            owner.HealthStat.OnDamaged += OnDamaged;
            owner.HealthStat.OnHealed += OnHealed;

            UpdateBuff();
        }

        private void OnHealed(float arg1, EffectSource arg2, EffectProperty arg3) => UpdateBuff();

        private void OnDamaged(float arg1, EffectSource arg2, EffectProperty arg3) => UpdateBuff();

        private void UpdateBuff()
        {
            var healthPercent = owner.HealthStat.CurrentValue / owner.HealthStat.TotalValue;
            var buffValue = (1 - healthPercent) * dataConfigItem.buffStat.value;

            owner.DebuffStat(dataConfigItem.buffStat.statType, _currentBuffValue, dataConfigItem.buffStat.statModifyType);
            owner.BuffStat(dataConfigItem.buffStat.statType, buffValue, dataConfigItem.buffStat.statModifyType);
            _currentBuffValue = buffValue;
        }
    }
}