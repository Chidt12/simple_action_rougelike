using Cysharp.Threading.Tasks;
using Runtime.Core.Message;
using Runtime.Gameplay.EntitySystem;
using Runtime.Manager;
using Runtime.Manager.Gameplay;
using Runtime.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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

        [Header("Boss Introduce")]
        [SerializeField] private EntityIntroduceUI _entityIntroduceUI;

        [Header("Artifact display")]
        [SerializeField] private RuneArtifactCooldownIcon _runeArtifactIconPrefab;
        [SerializeField] private Transform _runeArtifactIconsContainer;
        [SerializeField] private StackArtifactIcon _stackArtifactIconPrefab;
        [SerializeField] private Transform _stackArtifactIconsContainer;
 
        private List<DashIcon> _activeDashIcons;
        private List<RuneArtifactCooldownIcon> _runeArtifactIcons;
        private List<StackArtifactIcon> _stackArtifactIcons;
        private bool _isIntroducing;

        private IEntityStatData _heroData;
        private List<ISubscription> _subscriptions;
        private EntityStatWithCurrentValue _dashStat;
        private CancellationTokenSource _cancellationTokenSource;

        public override UniTask Initialize(Memory<object> args)
        {
            _isIntroducing = false;
            _entityIntroduceUI.gameObject.SetActive(false);
            _activeDashIcons = new();
            _runeArtifactIcons = new();
            _stackArtifactIcons = new();
            _cancellationTokenSource = new();

            _subscriptions = new();
            _subscriptions.Add(SimpleMessenger.Subscribe<HeroSpawnedMessage>(OnHeroSpawned));
            _subscriptions.Add(SimpleMessenger.Subscribe<EntitySpawnedMessage>(OnEntitySpawned));
            _subscriptions.Add(SimpleMessenger.Subscribe<FinishedLoadNextLevelMessage>(OnLoadNextLevel));
            _subscriptions.Add(SimpleMessenger.Subscribe<UpdateCurrentArtifactMessage>(OnUpdateCurrentArtifact));
            _subscriptions.Add(SimpleMessenger.Subscribe<UpdateCurrentCollectedArtifactMessage>(OnUpdateCollectedArtifact));
            _subscriptions.Add(SimpleMessenger.Subscribe<EntityIntroducedMessage>(OnEntityIntroduced));

            InitializeStackArtifacts();
            InitializeRuneArtifacts();
            ResetBossHealthBars();
            return base.Initialize(args);
        }

        public override UniTask Cleanup()
        {
            foreach (var subscription in _subscriptions)
                subscription.Dispose();

            _subscriptions.Clear();

            ResetBossHealthBars();
            _cancellationTokenSource.Dispose();

            return base.Cleanup();
        }

        private void InitializeStackArtifacts()
        {
            foreach (Transform item in _stackArtifactIconsContainer)
                Destroy(item.gameObject);

            _stackArtifactIcons.Clear();

            for (int i = 0; i < GameplayManager.Instance.GameBalancingConfig.numberStackArtifact; i++)
            {
                var stackArtifact = Instantiate(_stackArtifactIconPrefab, _stackArtifactIconsContainer);
                stackArtifact.Clear();
                _stackArtifactIcons.Add(stackArtifact);
            }
        }

        private void InitializeRuneArtifacts()
        {
            foreach (Transform item in _runeArtifactIconsContainer)
                Destroy(item.gameObject);
        }


        private void OnEntityIntroduced(EntityIntroducedMessage message)
        {
            if (!GameplayManager.Instance.EntityIntroduced.Contains(message.EntityId))
            {
                if (_isIntroducing)
                {
                    GameplayManager.Instance.AddIntroduceToQueue(message);
                }
                else
                {
                    _isIntroducing = true;
                    StartIntroduce(message).Forget();
                }
            }
        }

        private async UniTaskVoid StartIntroduce(EntityIntroducedMessage message)
        {
            GameplayManager.Instance.AddIntroduced(message.EntityId);
            await UniTask.Delay(TimeSpan.FromSeconds(0.3f), cancellationToken: _cancellationTokenSource.Token);
            await _entityIntroduceUI.StartIntroduce(message.EntityId, message.EntityType, _cancellationTokenSource.Token);
            await UniTask.Delay(TimeSpan.FromSeconds(message.IntroduceTime), ignoreTimeScale: true, cancellationToken: _cancellationTokenSource.Token);
            _entityIntroduceUI.EndIntroduce(CheckToStartIntroduce); 
        }

        private void CheckToStartIntroduce()
        {
            var popResult = GameplayManager.Instance.PopIntroduceInQueue();
            if (popResult.Item2)
            {
                StartIntroduce(popResult.Item1).Forget();
            }
            else
            {
                _isIntroducing = false;
            }
        }

        private void OnUpdateCurrentArtifact(UpdateCurrentArtifactMessage message)
        {
            if(message.UpdateArtifactType == UpdateCurrentArtifactType.Added)
            {
                if(message.UpdatedArtifact is ICooldown)
                {
                    var runeArtifactIcon = Instantiate(_runeArtifactIconPrefab, _runeArtifactIconsContainer);
                    runeArtifactIcon.Init(message.UpdatedArtifact, _cancellationTokenSource.Token);
                    _runeArtifactIcons.Add(runeArtifactIcon);
                }
            }
            else if (message.UpdateArtifactType == UpdateCurrentArtifactType.Removed)
            {
                var icon = _runeArtifactIcons.FirstOrDefault(x => x.ArtifactType == message.UpdatedArtifact.ArtifactType);
                _runeArtifactIcons.Remove(icon);
                Destroy(icon.gameObject);
            }
        }

        private void OnUpdateCollectedArtifact(UpdateCurrentCollectedArtifactMessage message)
        {
            if(message.UpdatedCurrentCollectedArtifactType == UpdatedCurrentCollectedArtifactType.Add)
            {
                var lastItem = _stackArtifactIcons.LastOrDefault(x => x.HasData);
                if (lastItem)
                    lastItem.ToggleSelect(false);

                foreach (var item in _stackArtifactIcons)
                {
                    if (!item.HasData)
                    {
                        item.UpdateData(message.ArtifactType, message.DataId, _cancellationTokenSource.Token);
                        item.ToggleSelect(true);
                        break;
                    }
                }
            }
            else if (message.UpdatedCurrentCollectedArtifactType == UpdatedCurrentCollectedArtifactType.Used)
            {
                var lastItem = _stackArtifactIcons.LastOrDefault(x => x.HasData);
                if (lastItem)
                    lastItem.Clear();
                lastItem = _stackArtifactIcons.LastOrDefault(x => x.HasData);
                if (lastItem)
                    lastItem.ToggleSelect(true);
            }
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
                    InitDashIcons((int)_dashStat.TotalValue);
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

        private void InitDashIcons(int numberOfDash)
        {
            foreach (Transform item in _dashContainer)
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

        private void OnDashValueChanged(float value) => InitDashIcons((int)_dashStat.TotalValue);

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
