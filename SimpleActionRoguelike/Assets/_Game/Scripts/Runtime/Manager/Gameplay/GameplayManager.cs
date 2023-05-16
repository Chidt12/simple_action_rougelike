using Cysharp.Threading.Tasks;
using DG.Tweening;
using Runtime.ConfigModel;
using Runtime.Constants;
using Runtime.Core.Message;
using Runtime.Core.Pool;
using Runtime.Core.Singleton;
using Runtime.Definition;
using Runtime.Gameplay;
using Runtime.Gameplay.CollisionDetection;
using Runtime.Gameplay.EntitySystem;
using Runtime.Helper;
using Runtime.Manager.Data;
using Runtime.Message;
using Runtime.SceneLoading;
using Runtime.UI;
using System;
using System.Linq;
using System.Threading;
using UnityEngine;
using ZBase.Foundation.PubSub;
using ZBase.UnityScreenNavigator.Core.Views;
using TweenType = Runtime.Helper.TweenType;

namespace Runtime.Manager.Gameplay
{
    public class GameplayManager : MonoSingleton<GameplayManager>
    {
        protected string[] Levels = new[] { "Level1", "Level2" };

        protected int RewardCoins = 2;
        protected BuffInGameType[] BuffsInGameType = new[] { BuffInGameType.RotateOrbs };

        protected WaveTimer waveTimer;
        protected bool hasFinishedSpawnWave;
        protected int maxWaveIndex;
        private ISubscription _gameplayDataLoadedRegistry;
        private ISubscription _entityDiedRegistry;
        private ISubscription _goToNextLevelMapRegistry;
        private MapLevel _currentMapLevel;
        private CancellationTokenSource _cancellationTokenSource;

        private bool _isWinCurrentLevel;

        private StageLoadConfigItem StageInfo => DataManager.Config.GetStageConfigData(GameplayDataDispatcher.Instance.StageId);
        public int CurrentGameplayTimeInSecond => waveTimer.CurrentGameplayTime;
        public int CurrentWaveIndex { get; protected set; }

        #region API Methods

        protected override void Awake()
        {
            base.Awake();
            waveTimer = new WaveTimer();
            _cancellationTokenSource = new CancellationTokenSource();
            _gameplayDataLoadedRegistry = SimpleMessenger.Subscribe<GameplayDataLoadedMessage>(OnGameplayDataLoaded);
            _entityDiedRegistry = SimpleMessenger.Subscribe<EntityDiedMessage>(OnEntityDied);
            _goToNextLevelMapRegistry = SimpleMessenger.Subscribe<GoToNextLevelMapMessage>(OnGoToNextLevelMap);
            SceneLoaderManager.RegisterScenePreloadedAction(Dispose);
        }

        #endregion API Methods

        #region Class Methods

        public async UniTaskVoid ShowShop()
        {
            var shopItems = await ConfigDataManager.Instance.LoadCurrentSuitableShopInGameItems();
            var selectInGameShopData = new ModalSelectIngameShopData(shopItems.ToArray(), OnBuyShopItem);
            await ScreenNavigator.Instance.LoadModal(new WindowOptions(ModalIds.SELECT_INGAME_SHOP), selectInGameShopData);
        }

        private void OnBuyShopItem(ShopInGameStageLoadConfigItem item)
        {
            if(item.cost.resourceType == ResourceType.MoneyInGame)
            {
                var value = DataManager.Transient.GetGameMoneyType((InGameMoneyType)item.cost.resourceId);
                if(value < item.cost.resourceNumber)
                {
                    ToastController.Instance.Show("Not Enough Resource!");
                    return;
                }

                DataManager.Transient.RemoveMoney((InGameMoneyType)item.cost.resourceId, item.cost.resourceNumber);

                AddShopItemAsync(item.identity).Forget();
            }
        }

        private async UniTask AddShopItemAsync(ShopInGameIdentity identity)
        {
            await ShopInGameManager.Instance.AddShopInGameItem(EntitiesManager.Instance.HeroData, identity.shopInGameItemType, identity.dataId);
            var dataConfig = await ConfigDataManager.Instance.LoadShopInGameDataConfig(identity.shopInGameItemType);
            var description = await dataConfig.GetDescription(identity.dataId);
            ToastController.Instance.Show(description);
        }

        private void OnGameplayDataLoaded(GameplayDataLoadedMessage gameplayDataLoadedMessage)
        {
            EntitiesManager.Instance.Initialize();
            StartGameplay().Forget();
        }

        private void OnEntityDied(EntityDiedMessage entityDiedMessage)
        {
            var handleCharacterDiedResult = EntitiesManager.Instance.HandleCharacterDied(entityDiedMessage);
            if (handleCharacterDiedResult == HandleCharacterDiedResultType.DeletedAllEnemyOnMap)
                DeletedAllEnemyOnMap();
            else if (handleCharacterDiedResult == HandleCharacterDiedResultType.HeroDied)
                KillHero();
        }

        private void OnGoToNextLevelMap(GoToNextLevelMapMessage message)
        {
            if (_isWinCurrentLevel)
            {
                _isWinCurrentLevel = false;
                //Load Next Level
                LoadNextLevelAsync().Forget();
            }
        }

        private async UniTaskVoid LoadNextLevelAsync()
        {
            // Fade In
            var fadeTween = new TweenType(TweenHelper.TweenCurve.EaseInSinusoidal);
            SimpleMessenger.Publish(new FadeOutMessage(0.75f, fadeTween, false, EntitiesManager.Instance.HeroData.Position, false));
            await UniTask.Delay(TimeSpan.FromSeconds(0.75f), cancellationToken: _cancellationTokenSource.Token);

            await LoadLevelAsync();
            EntitiesManager.Instance.HeroData.ForceUpdatePosition.Invoke(MapManager.Instance.SpawnPoints[0].transform.position);

            // Delay to wait for camera move to hero.
            await UniTask.Delay(TimeSpan.FromSeconds(0.75f), cancellationToken: _cancellationTokenSource.Token);
            // Fade Out
            SimpleMessenger.Publish(new FadeInMessage(0.5f, fadeTween, false, EntitiesManager.Instance.HeroData.Position, true));
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: _cancellationTokenSource.Token);
            // TODO JUST FOR TEST LOAD MAP
            CurrentWaveIndex = 0;
            waveTimer.SetUp();
            StartWave();
        }

        private async UniTaskVoid StartGameplay()
        {
            // Load Level
            await LoadLevelAsync();
            // Load Hero.
            await EntitiesManager.Instance.CreateEntityAsync("1001", 
                GameplayDataDispatcher.Instance.HeroLevel, 
                EntityType.Hero, 
                MapManager.Instance.SpawnPoints[0].transform.position,
                cancellationToken: _cancellationTokenSource.Token);

            var stageInfo = StageInfo;
            waveTimer.SetUp();
            maxWaveIndex = stageInfo.waveConfigs.Max(x => x.waveIndex);
            StartWave();
        }

        private async UniTask LoadLevelAsync()
        {
            // Load Level Graphic
            var newLevel = await PoolManager.Instance.Rent(Levels[UnityEngine.Random.Range(0, Levels.Length)], token: _cancellationTokenSource.Token);
            if (_currentMapLevel)
                PoolManager.Instance.Return(_currentMapLevel.gameObject);

            _currentMapLevel = null;
            _currentMapLevel = newLevel.GetOrAddComponent<MapLevel>();
            CameraManager.Instance.SetConfinder(_currentMapLevel.confinder);
            MapManager.Instance.LoadLevelMap(_currentMapLevel);
        }

        private void StartWave()
        {
            hasFinishedSpawnWave = false;
            var stageInfo = StageInfo;
            EntitiesManager.Instance.SpawnStageWave(stageInfo, CurrentWaveIndex, OnFinishSpawnWave);
            var waveConfig = stageInfo.waveConfigs.FirstOrDefault(x => x.waveIndex == CurrentWaveIndex);
            waveTimer.Start(waveConfig, onFinish: () => FinishWave(false));
            SimpleMessenger.Publish(new WaveTimeUpdatedMessage(true, CurrentGameplayTimeInSecond, CurrentWaveIndex, maxWaveIndex));
        }

        private void OnFinishSpawnWave()
        => hasFinishedSpawnWave = true;

        protected virtual void FinishWave(bool isClearWave)
        {
            var highestWaveIndex = maxWaveIndex;
            if (CurrentWaveIndex == highestWaveIndex)
            {
                if (isClearWave)
                {
                    HandleWinLevelAsync().Forget();
                }
                else
                {
                    var hasNoEnemiesLeft = EntitiesManager.Instance.HaveNoEnemiesLeft;
                    if (hasNoEnemiesLeft)
                        HandleWinLevelAsync().Forget();
                    else
                        HandleLoseStage();
                }
            }
            else
            {
                CurrentWaveIndex += 1;
                StartWave();
            }
        }

        private void DeletedAllEnemyOnMap()
        {
            if (hasFinishedSpawnWave)
                FinishWave(true);
        }

        private void KillHero()
        {
            ScreenNavigator.Instance.LoadModal(new WindowOptions(ModalIds.LOSE)).Forget();
        }

        private async UniTask HandleWinLevelAsync()
        {
            _isWinCurrentLevel = true;

            ToastController.Instance.Show($"Add + {RewardCoins}");
            DataManager.Transient.AddMoney(InGameMoneyType.Gold, RewardCoins);

            // Pause for select buff
            GameManager.Instance.SetGameStateType(GameStateType.GameplayPausing);

            var heroEntityData = EntitiesManager.Instance.HeroData;
            var currentBuffs = MechanicSystemManager.Instance.GetCurrentBuffsInGame();

            var suitableItems = await DataManager.Config.LoadCurrentSuitableBuffInGameItems(currentBuffs);

            var modalData = new ModalSelectIngameBuffData(heroEntityData, suitableItems.Select(x => x.identity).ToArray(), OnSelectBuffItem);
            ScreenNavigator.Instance.LoadModal(new WindowOptions(ModalIds.SELECT_INGAME_BUFF), modalData).Forget();
        }

        private void OnSelectBuffItem(BuffInGameIdentity dataIdentity)
        {
            OnSelectBuffItemAsync(dataIdentity).Forget();
        }

        private async UniTaskVoid OnSelectBuffItemAsync(BuffInGameIdentity dataIdentity)
        {
            var heroData = EntitiesManager.Instance.HeroData;
            await MechanicSystemManager.Instance.AddBuffInGameSystem(heroData, dataIdentity.buffInGameType);
            GameManager.Instance.ReturnPreviousGameStateType();
            await ScreenNavigator.Instance.PopModal(true);
        }

        private void HandleLoseStage()
        {
            Debug.LogError("LOSE GAME");
        }

        public void Dispose()
        {
            _gameplayDataLoadedRegistry.Dispose();
            _entityDiedRegistry.Dispose();
            _goToNextLevelMapRegistry.Dispose();

            var disposables = FindObjectsOfType<Disposable>();
            foreach (var dispose in disposables)
                dispose.Dispose();

            MechanicSystemManager.Instance.Dispose();
            CollisionSystem.Instance.Dispose();
            EntitiesManager.Instance.Dipose();
            MapManager.Instance.Dispose();
        }

        #endregion Class Methods
    }

}