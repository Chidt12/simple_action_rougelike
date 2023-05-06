using Cysharp.Threading.Tasks;
using Runtime.Definition;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    [DisallowMultipleComponent]
    public class EntityDisplayHUDBehavior : EntityBehavior<IEntityStatData>
    {
        [SerializeField]
        private EntityHUD _entityHUD;

        private IEntityStatData _entityStatData;

#if UNITY_EDITOR
        private void OnValidate()
        {
            _entityHUD = GetComponentInChildren<EntityHUD>();
        }
#endif

        protected override UniTask<bool> BuildDataAsync(IEntityStatData data)
        {
            if (_entityHUD == null || data == null)
                return UniTask.FromResult(false);

            _entityStatData = data;

            if (_entityStatData.HealthStat != null)
            {
                _entityStatData.HealthStat.OnDamaged += OnHealthChanged;
                _entityStatData.HealthStat.OnHealed += OnHealthChanged;
                _entityStatData.HealthStat.OnValueChanged += OnHealthValueChanged;
            }

            if (_entityStatData.ShieldStat != null)
            {
                _entityStatData.ShieldStat.OnValueChanged += OnShieldValueChanged;
                _entityHUD.Init(_entityStatData.ShieldStat.CurrentValue, _entityStatData.ShieldStat.TotalValue);
            }
            else
            {
                _entityHUD.Init(0, 0);
            }
            var resultGetShield = data.TryGetStat(StatType.Shield, out var shieldStat);
            return UniTask.FromResult(true);
        }

        private void OnShieldValueChanged(float value) => Display();
        private void OnHealthValueChanged(float value) => Display();

        private void OnHealthChanged(float deltaHp, EffectSource effectSource, EffectProperty effectProperty) => Display();

        private void Display()
        {
            if (_entityStatData.ShieldStat != null)
                _entityHUD.UpdateShieldBar(_entityStatData.ShieldStat.CurrentValue, _entityStatData.ShieldStat.TotalValue);
            if (_entityStatData.HealthStat != null)
                _entityHUD.UpdateHealthBar(_entityStatData.HealthStat.CurrentValue, _entityStatData.HealthStat.TotalValue);
        }

    }
}