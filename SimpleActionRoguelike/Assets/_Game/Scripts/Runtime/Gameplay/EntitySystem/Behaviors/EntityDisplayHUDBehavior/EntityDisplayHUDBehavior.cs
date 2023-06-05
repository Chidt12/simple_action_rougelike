using Cysharp.Threading.Tasks;
using Runtime.Definition;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    [DisallowMultipleComponent]
    public class EntityDisplayHUDBehavior : EntityBehavior<IEntityStatData>, IDisposeEntityBehavior
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
                _entityStatData.HealthStat.OnDamaged += OnDamaged;
                _entityStatData.HealthStat.OnHealed += OnHealed;
                _entityStatData.HealthStat.OnValueChanged += OnHealthValueChanged;

                if (_entityStatData.HealthStat != null)
                    _entityHUD.Init(_entityStatData.HealthStat.CurrentValue, _entityStatData.HealthStat.TotalValue);
            }

            return UniTask.FromResult(true);
        }

        private void OnHealthValueChanged(float value) => Display(false);

        private void OnDamaged(float deltaHp, EffectSource effectSource, EffectProperty effectProperty) => Display(true);

        private void OnHealed(float deltaHp, EffectSource effectSource, EffectProperty effectProperty) => Display(false);

        private void Display(bool isHurt)
        {
            if (_entityStatData.HealthStat != null)
                _entityHUD.UpdateHealthBar(_entityStatData.HealthStat.CurrentValue, _entityStatData.HealthStat.TotalValue, isHurt);
        }

        public void Dispose()
        {
            _entityHUD.Dispose();
        }
    }
}