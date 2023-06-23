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
        [SerializeField] private BossHealthBar[] _bossHealthBars;

        private List<DashIcon> _activeDashIcons;

        private IEntityStatData _heroData;
        private List<ISubscription> _subscriptions;
        private EntityStatWithCurrentValue _dashStat;

        public override UniTask Initialize(Memory<object> args)
        {
            _activeDashIcons = new();
            _subscriptions = new();
            _subscriptions.Add(SimpleMessenger.Subscribe<HeroSpawnedMessage>(OnHeroSpawned));
            _subscriptions.Add(SimpleMessenger.Subscribe<EntitySpawnedMessage>(OnEntitySpawned));
            _subscriptions.Add(SimpleMessenger.Subscribe<FinishedLoadNextLevelMessage>(OnLoadNextLevel));
            ResetBossHealthBars();
            return base.Initialize(args);
        }

        private void OnLoadNextLevel(FinishedLoadNextLevelMessage message) => ResetBossHealthBars();

        private void ResetBossHealthBars()
        {
            foreach (var bossHealthBar in _bossHealthBars)
            {
                bossHealthBar.Dispose();
                bossHealthBar.gameObject.SetActive(false);
            }
        }

        private void OnEntitySpawned(EntitySpawnedMessage message)
        {
            if(message.EntityData.EntityType == Definition.EntityType.Boss)
            {
                foreach (var bossHealthBar in _bossHealthBars)
                {
                    if (bossHealthBar.IsAvailable)
                    {
                        bossHealthBar.gameObject.SetActive(true);
                        bossHealthBar.Init((IEntitySkillData)message.EntityData, (IEntityStatData)message.EntityData);
                        break;
                    }
                }
            }
        }

        public override UniTask Cleanup()
        {
            foreach (var subscription in _subscriptions)
            {
                subscription.Dispose();
            }
            _subscriptions.Clear();

            foreach (var bossHealthBar in _bossHealthBars)
                bossHealthBar.Dispose();
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
                Destroy(item.gameObject);

            _activeDashIcons.Clear();

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
