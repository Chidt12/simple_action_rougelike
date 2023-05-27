using Cysharp.Threading.Tasks;
using DG.Tweening;
using Runtime.ConfigModel;
using Runtime.Constants;
using Runtime.Core.Message;
using Runtime.Core.Pool;
using Runtime.Core.Singleton;
using Runtime.Definition;
using Runtime.Gameplay;
using Runtime.Gameplay.Balancing;
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
using Random = UnityEngine.Random;
using TweenType = Runtime.Helper.TweenType;

namespace Runtime.Manager.Gameplay
{
    public class GameplayManager : MonoSingleton<GameplayManager>
    {
        protected int RewardCoins = 2;

        protected WaveTimer waveTimer;
        protected bool hasFinishedSpawnWave;
        protected int maxWaveIndex;
        private ISubscription _gameplayDataLoadedRegistry;
        private ISubscription _entityDiedRegistry;
        private ISubscription _goToNextLevelMapRegistry;
        private CancellationTokenSource _cancellationTokenSource;
        private bool _isWinCurrentLevel;

        private MapLevel _currentMapLevel;
        private StageLoadConfigItem _currentStageLoadConfigItem;
        private int _stageNumber;

        public GameBalancingConfig GameBalancingConfig => GameplayDataManager.Instance.GetGameBalancingConfig();
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
            GameManager.Instance.SetGameStateType(GameStateType.GameplayPausing);

            // Fade In
            var fadeTween = new TweenType(TweenHelper.TweenCurve.EaseInSinusoidal);
            SimpleMessenger.Publish(new FadeOutMessage(0.75f, fadeTween, true, EntitiesManager.Instance.HeroData.Position, false));
            await UniTask.Delay(TimeSpan.FromSeconds(0.75f), cancellationToken: _cancellationTokenSource.Token, ignoreTimeScale: true);

            // Load Level.
            _stageNumber++;
            await LoadLevelAsync();
            EntitiesManager.Instance.HeroData.ForceUpdatePosition.Invoke(MapManager.Instance.SpawnPoints[0].transform.position);

            // Delay to wait for camera move to hero.
            await UniTask.Delay(TimeSpan.FromSeconds(0.75f), cancellationToken: _cancellationTokenSource.Token, ignoreTimeScale: true);
            // Fade Out
            GameManager.Instance.ReturnPreviousGameStateType();
            SimpleMessenger.Publish(new FadeInMessage(0.5f, fadeTween, true, EntitiesManager.Instance.HeroData.Position, true));
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: _cancellationTokenSource.Token, ignoreTimeScale: true);

            SetUpNewStage();
        }

        private async UniTaskVoid StartGameplay()
        {
            // Load Level
            _stageNumber = 0;
            await LoadLevelAsync();

            // Load Hero.
            await EntitiesManager.Instance.CreateEntityAsync(
                new SpawnedEntityInfo(Constant.HERO_ID, EntityType.Hero, GameplayDataDispatcher.Instance.HeroLevel), 
                MapManager.Instance.SpawnPoints[0].transform.position,
                cancellationToken: _cancellationTokenSource.Token);

            SetUpNewStage();
        }

        private void SetUpNewStage()
        {
            var stageInfo = _currentStageLoadConfigItem;
            if (stageInfo.waveConfigs != null && stageInfo.waveConfigs.Length > 0)
            {
                _isWinCurrentLevel = false;
                CurrentWaveIndex = 0;
                waveTimer.SetUp();
                maxWaveIndex = stageInfo.waveConfigs.Max(x => x.waveIndex);
                StartWave();
            }
            else
            {
                _isWinCurrentLevel = true;
            }
        }

        private async UniTask LoadLevelAsync()
        {
            // Destroy current level

            if (_currentMapLevel)
                Destroy(_currentMapLevel.gameObject);

            await UniTask.Delay(TimeSpan.FromSeconds(0.1f), cancellationToken: _cancellationTokenSource.Token, ignoreTimeScale: true);

            // Load Level Graphic
            var gameBalancingConfig = GameBalancingConfig;
            var heroPoint = CalculateHeroPoint();
            var (newLevel, stageLoadConfig, result) = await gameBalancingConfig.GetNextStage(heroPoint, _stageNumber);

            _currentStageLoadConfigItem = stageLoadConfig;
            _currentMapLevel = Instantiate(newLevel);
            CameraManager.Instance.SetConfinder(_currentMapLevel.confinder);
            MapManager.Instance.LoadLevelMap(_currentMapLevel);
        }

        private int CalculateHeroPoint()
        {
            var heroPoint = 0;

            var heroData = EntitiesManager.Instance.HeroData;
            // Hero Stats;
            heroPoint += 10; // Dummy;

            // Bonus weapon point
            var weaponData = heroData as IEntityWeaponData;
            if(weaponData != null)
                heroPoint += weaponData.WeaponModel.BalancingPoint;

            // Bonus shop point
            var allShopItems = ShopInGameManager.Instance.CurrentShopInGameItems;
            foreach (var shopItem in allShopItems)
                heroPoint += shopItem.BalancingPoint;

            return heroPoint;
        }

        private void StartWave()
        {
            hasFinishedSpawnWave = false;
            EntitiesManager.Instance.SpawnStageWave(_currentStageLoadConfigItem, CurrentWaveIndex, OnFinishSpawnWave);
            var waveConfig = _currentStageLoadConfigItem.waveConfigs.FirstOrDefault(x => x.waveIndex == CurrentWaveIndex);
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