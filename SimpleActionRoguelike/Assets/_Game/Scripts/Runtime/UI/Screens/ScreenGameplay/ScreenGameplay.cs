using Cysharp.Threading.Tasks;
using Runtime.Core.Message;
using Runtime.Gameplay.EntitySystem;
using Runtime.Message;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ZBase.Foundation.PubSub;
using Screen = ZBase.UnityScreenNavigator.Core.Screens.Screen;

namespace Runtime.UI
{
    public class ScreenGameplay : Screen
    {
        [SerializeField] private Image _progress;
        [SerializeField] private TextMeshProUGUI _progressText;
        [SerializeField] private Animator _hurtAnimator;
        [SerializeField] private DashIcon _dashIconPrefab;
        [SerializeField] private Transform _dashContainer;

        private List<DashIcon> _activeDashIcons;

        private IEntityStatData _heroData;
        private ISubscription _subscription;
        private EntityStatWithCurrentValue _dashStat;

        public override UniTask Initialize(Memory<object> args)
        {
            _activeDashIcons = new();
            _subscription = SimpleMessenger.Subscribe<HeroSpawnedMessage>(OnHeroSpawned);
            return base.Initialize(args);
        }

        public override UniTask Cleanup()
        {
            _subscription.Dispose();
            return base.Cleanup();
        }

        private void OnHeroSpawned(HeroSpawnedMessage message)
        {
            _heroData = (IEntityStatData)message.EntityData;
            if(_heroData != null)
            {
                // update health visual
                _heroData.HealthStat.OnDamaged += OnDamage;
                _heroData.HealthStat.OnHealed += OnHeal;
                UpdateHud();

                // update dash visual
                if(_heroData.TryGetStat(Definition.StatType.DashNumber, out var dash))
                {
                    _dashStat = dash as EntityStatWithCurrentValue;
                    _dashStat.OnValueChanged += OnDashValueChanged;
                    _dashStat.OnIncreaseCurrentValue += OnDashInCreaseCurrentValue;
                    _dashStat.OnDecreaseCurrentValue += OnDashDecreaseCurrentValue;
                    InitIcons((int)_dashStat.TotalValue);
                }
            }
        }

        private void OnDashDecreaseCurrentValue(float obj) => UpdateCurrentDashValue(false);

        private void OnDashInCreaseCurrentValue(float obj) => UpdateCurrentDashValue(true);

        private void UpdateCurrentDashValue(bool updateAnimation)
        {
            for (int i = 0; i < _activeDashIcons.Count; i++)
            {
                if (i < _dashStat.CurrentValue)
                    _activeDashIcons[i].ToggleActive(true, updateAnimation);
                else
                    _activeDashIcons[i].ToggleActive(false, updateAnimation);
            }
        }

        private void InitIcons(int numberOfDash)
        {
            foreach (var item in _activeDashIcons)
                Destroy(item);

            for (int i = 0; i < numberOfDash; i++)
            {
                var dash = Instantiate(_dashIconPrefab, _dashContainer);
                if(i < _dashStat.CurrentValue)
                    dash.ToggleActive(true, true);
                else
                    dash.ToggleActive(false, true);
                _activeDashIcons.Add(dash);
            }
        }

        private void OnDashValueChanged(float value) => InitIcons((int)_dashStat.TotalValue);

        private void OnDamage(float value, EffectSource effectSource, EffectProperty effectProperty)
        {
            if (value > 0)
            {
                _hurtAnimator.Play("play", 0, 0);
            }
            UpdateHud();
        }

        private void OnHeal(float value, EffectSource effectSource, EffectProperty effectProperty)
        {
            UpdateHud();
        }

        private void UpdateHud()
        {
            _progressText.text = $"{Mathf.FloorToInt(_heroData.HealthStat.CurrentValue)}/{_heroData.HealthStat.TotalValue}";
            _progress.fillAmount = _heroData.HealthStat.CurrentValue / _heroData.HealthStat.TotalValue;
        }
    }
}
