using Cysharp.Threading.Tasks;
using Runtime.Core.Message;
using Runtime.Gameplay.EntitySystem;
using Runtime.Message;
using System;
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

        private IEntityStatData _heroData;
        private ISubscription _subscription;

        public override UniTask Initialize(Memory<object> args)
        {
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
                _heroData.HealthStat.OnDamaged += OnDamage;
                _heroData.HealthStat.OnHealed += OnHeal;
                UpdateHud();
            }
        }

        private void OnDamage(float value, EffectSource effectSource, EffectProperty effectProperty)
        {
            UpdateHud();
        }

        private void OnHeal(float value, EffectSource effectSource, EffectProperty effectProperty)
        {
            UpdateHud();
        }

        private void UpdateHud()
        {
            _progressText.text = $"{_heroData.HealthStat.CurrentValue}/{_heroData.HealthStat.TotalValue}";
            _progress.fillAmount = _heroData.HealthStat.CurrentValue / _heroData.HealthStat.TotalValue;
        }
    }
}
