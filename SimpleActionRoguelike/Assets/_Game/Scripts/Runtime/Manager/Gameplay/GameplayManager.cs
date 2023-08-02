using Cysharp.Threading.Tasks;
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
using Runtime.Gameplay.Manager;
using Runtime.Helper;
using Runtime.Manager.Audio;
using Runtime.Manager.Data;
using Runtime.Message;
using Runtime.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using ZBase.Foundation.PubSub;
using ZBase.UnityScreenNavigator.Core.Views;
using TweenType = Runtime.Helper.TweenType;

namespace Runtime.Manager.Gameplay
{

    public abstract class CheckEndStage : MonoBehaviour
    {
        public abstract bool IsAvailableForEndStage { get; }
    }

    public class GameplayManager : MonoSingleton<GameplayManager>
    {
        [SerializeField] private ArtifactChestItem _artifactChest;
        [SerializeField] private ShopChestItem _shopChest;
        [SerializeField] private CameraManager _cameraManager;

        [Header("==== Sounds ====")]
        [SerializeField] private string _heroAppear;

        public const int NUMBER_OF_SELECT_ARTIFACT = 3;
        public const int NUMBER_OF_SELECT_SHOP_ITEM = 3;
        public const int RESET_COST = 5;
        protected MechanicSystemManager mechanicSystemManager;
        protected ShopInGameManager shopInGameManager;
        protected GameplayMessageCenter messageCenter;
        protected WaveTimer waveTimer;
        private List<ISubscription> subscriptions;
        private List<IDisposable> otherDisposables;

        protected int RewardCoins = 15;

        protected bool hasFinishedSpawnWave;
        protected int maxWaveIndex;
        private CancellationTokenSource _cancellationTokenSource;
        private bool _isWinCurrentLevel;

        private MapLevel _currentLevelMap;
        private StageLoadConfigItem _currentStageLoadConfigItem;
        private CurrentLoadedStageData _currentStageData;
        private List<CheckEndStage> _checkEndStageConditions;
        private Queue<EntityIntroducedMessage> _entityWaitingToIntroducQueue;
        private List<int> _entityIntroduced;

        private bool _isLoadedShopItems;
        private List<ShopInGameStageLoadConfigItem> _shopItems;

        public Queue<EntityIntroducedMessage> EntityWaitingToIntroducQueue => _entityWaitingToIntroducQueue;
        public List<int> EntityIntroduced => _entityIntroduced;
        public GameplayMessageCenter MessageCenter => messageCenter;
        public MechanicSystemManager MechanicSystemManager => mechanicSystemManager;
        public ShopInGameManager ShopInGameManager => shopInGameManager;
        public List<ArtifactIdentity> CurrentBuffInGameItems => mechanicSystemManager.GetCurrentBuffsInGame();
        public List<ShopInGameItem> CurrentShopInGameItems => shopInGameManager.CurrentShopInGameItems;
        public CurrentLoadedStageData CurrentStageData => _currentStageData;
        public GameBalancingConfig GameBalancingConfig => GameplayDataManager.Instance.GetGameBalancingConfig();
        public int CurrentGameplayTimeInSecond => waveTimer.CurrentGameplayTime;
        public int CurrentWaveIndex { get; protected set; }

        #region Class Methods

        public async UniTask InitAsync()
        {
            _isLoadedShopItems = false;

            otherDisposables = new();
            subscriptions = new()
            {
                SimpleMessenger.Subscribe<EntityNotifyDiedMessage>(OnEntityDied),
                SimpleMessenger.Subscribe<SendToGameplayMessage>(OnReceiveMessage),
                SimpleMessenger.Subscribe<HeroSpawnedMessage>(OnHeroSpawned)
            };


            _cameraManager.Init();

            mechanicSystemManager = new();
            mechanicSystemManager.Init();

            shopInGameManager = new();
            shopInGameManager.Init();

            messageCenter = new();
            messageCenter.Init();

            waveTimer = new WaveTimer();
            _checkEndStageConditions = new();
            _cancellationTokenSource = new CancellationTokenSource();

            EntitiesManager.Instance.Initialize();
            await StartGameplayAsync();
        }

        public void Dispose()
        {
            _cancellationTokenSource?.Cancel();

            foreach (var subscription in subscriptions)
                subscription.Dispose();
            subscriptions.Clear();

            foreach (var disposable in otherDisposables)
                disposable.Dispose();
            otherDisposables.Clear();

            var disposables = FindObjectsOfType<Disposable>();
            foreach (var dispose in disposables)
                dispose.Dispose();

            shopInGameManager.Dispose();
            mechanicSystemManager.Dispose();
            messageCenter.Dispose();
            waveTimer.Dispose();
            _cameraManager.Dispose();

            EntitiesManager.Instance.Dipose();
            MapManager.Instance.Dispose();
            CollisionSystem.Instance.Dispose();

            // Clear Map
            if (_checkEndStageConditions != null)
                foreach (var checkEndStage in _checkEndStageConditions)
                    Destroy(checkEndStage.gameObject);

            if (_currentLevelMap)
            {
                Destroy(_currentLevelMap.gameObject);
                _currentLevelMap = null;
            }

            // Clear Disposables
            foreach (var disposable in disposables)
            {
                if (disposable)
                    PoolManager.Instance.Return(disposable.gameObject);
            }
        }

        private async UniTask StartGameplayAsync()
        {
            // Load Level
            _currentStageData = new();

            if (DataManager.Local.playerBasicLocalData.CheckCompletedTut(TutorialType.GuideGameplay))
            {
                await LoadLevelAsync(GameplayRoomType.Lobby);
            }
            else
            {
                await LoadLevelAsync(GameplayRoomType.TutorialStage);
            }

            // Load Hero.
            await EntitiesManager.Instance.CreateEntityAsync(
                new SpawnedEntityInfo(Constant.HERO_ID, EntityType.Hero, GameplayDataDispatcher.Instance.HeroLevel),
                MapManager.Instance.SpawnPoints[0].transform.position,
                cancellationToken: _cancellationTokenSource.Token);

            SetUpNewStage();
        }

        private void OnHeroSpawned(HeroSpawnedMessage message)
        {
        }

        private void OnEntityDied(EntityNotifyDiedMessage entityDiedMessage)
        {
            OnEntityDiedAsync(entityDiedMessage).Forget();
        }

        private async UniTask OnEntityDiedAsync(EntityNotifyDiedMessage entityDiedMessage)
        {
            var handleCharacterDiedResult = await EntitiesManager.Instance.HandleCharacterDied(entityDiedMessage);
            if (handleCharacterDiedResult == HandleCharacterDiedResultType.DeletedAllEnemyOnMap)
                DeletedAllEnemyOnMap();
            else if (handleCharacterDiedResult == HandleCharacterDiedResultType.HeroDied)
                KillHero();
        }

        private void OnReceiveMessage(SendToGameplayMessage message)
        {
            if (message.SendToGameplayType == SendToGameplayType.GoNextStage)
            {
                if (_isWinCurrentLevel)
                {
                    var roomType = (GameplayRoomType)message.Data;
                    LoadNextLevelAsync(roomType).Forget();
                }
            }
            else
            {
                var availableToEndStage = true;
                foreach (var checkEndStage in _checkEndStageConditions)
                {
                    if (!checkEndStage.IsAvailableForEndStage)
                    {
                        availableToEndStage = false;
                        break;
                    }
                }

                if (availableToEndStage)
                {
                    OpenNextLevel();
                }

                if (message.SendToGameplayType == SendToGameplayType.BuyShop)
                {
                    LoadShopInGameAsync().Forget();
                }
                else if (message.SendToGameplayType == SendToGameplayType.GiveArtifact)
                {
                    LoadGiveArtifactAsync().Forget();
                }
                else if (message.SendToGameplayType == SendToGameplayType.GiveShopItem)
                {
                    LoadGiveShopAsync().Forget();
                }
            }
        }

        private async UniTaskVoid LoadGiveShopAsync()
        {
            var shopType = CurrentStageData.CurrentRoomType == GameplayRoomType.ElitePower ? ShopItemCategoryType.Power : ShopItemCategoryType.Speed;
            var shopItems = await ConfigDataManager.Instance.LoadCurrentSuitableShopInGameItems(shopInGameManager.CurrentShopInGameItems, NUMBER_OF_SELECT_SHOP_ITEM, shopType);
            var selectInGameShopData = new ModalGiveInGameShopData(shopItems.ToArray(), OnGiveShopItem);
            await ScreenNavigator.Instance.LoadModal(new WindowOptions(ModalIds.GIVE_INGAME_SHOP, false), selectInGameShopData);
        }

        private async UniTaskVoid LoadShopInGameAsync()
        {
            if(!_isLoadedShopItems)
            {
                _isLoadedShopItems = true;
                _shopItems = await ConfigDataManager.Instance.LoadCurrentSuitableShopInGameItems(shopInGameManager.CurrentShopInGameItems, NUMBER_OF_SELECT_SHOP_ITEM, ShopItemCategoryType.Both);
            }

            if(_shopItems.Count <= 0)
            {
                ToastController.Instance.Show("Sold out!");
            }
            else
            {
                var selectInGameShopData = new ModalBuyIngameShopData(_shopItems, OnBuyShopItem);
                await ScreenNavigator.Instance.LoadModal(new WindowOptions(ModalIds.BUY_INGAME_SHOP, false), selectInGameShopData);
            }
        }

        private bool OnBuyShopItem(ShopInGameStageLoadConfigItem item)
        {
            if (item.cost.resourceType == ResourceType.MoneyInGame)
            {
                var value = DataManager.Transient.GetGameMoneyType((InGameMoneyType)item.cost.resourceId);
                if (value < item.cost.resourceNumber)
                {
                    ToastController.Instance.Show("Not Enough Resource!");
                    return false;
                }

                DataManager.Transient.RemoveMoney((InGameMoneyType)item.cost.resourceId, item.cost.resourceNumber);

                AddShopItemAsync(item.identity).Forget();
                return true;
            }

            return false;
        }

        private void OnGiveShopItem(ShopInGameStageLoadConfigItem item)
        {
            AddShopItemAsync(item.identity).Forget();
        }

        private async UniTask AddShopItemAsync(ShopInGameIdentity identity)
        {
            await shopInGameManager.AddShopInGameItem(EntitiesManager.Instance.HeroData as IEntityModifiedStatData, identity.shopInGameItemType, identity.dataId);
            var dataConfig = await ConfigDataManager.Instance.LoadShopInGameDataConfig(identity.shopInGameItemType);
            var (title, description) = await dataConfig.GetDescription(identity.dataId);
            ToastController.Instance.Show("Received " + title);
        }

        private async UniTaskVoid LoadGiveArtifactAsync()
        {
            var heroEntityData = EntitiesManager.Instance.HeroData;
            var currentBuffs = mechanicSystemManager.GetCurrentBuffsInGame();

            var suitableItems = await DataManager.Config.LoadCurrentSuitableArtifactItems(currentBuffs, NUMBER_OF_SELECT_ARTIFACT);

            var modalData = new ModalGiveArtifactData(heroEntityData, suitableItems.Select(x => x.identity).ToArray(), OnSelectBuffItem);
            ScreenNavigator.Instance.LoadModal(new WindowOptions(ModalIds.GIVE_ARTIFACT, false), modalData).Forget();
        }

        private async UniTaskVoid LoadNextLevelAsync(GameplayRoomType roomType)
        {
            GameManager.Instance.SetGameStateType(GameStateType.GameplayPausing);

            // Fade In
            var fadeTween = new TweenType(TweenHelper.TweenCurve.EaseInSinusoidal);
            SimpleMessenger.Publish(new FadeOutMessage(0.5f, fadeTween, true, EntitiesManager.Instance.HeroData.Position, false));
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: _cancellationTokenSource.Token, ignoreTimeScale: true);

            // Load Level.
            await LoadLevelAsync(roomType);
            EntitiesManager.Instance.HeroData.ForceUpdatePosition.Invoke(MapManager.Instance.SpawnPoints[0].transform.position, true);
            SimpleMessenger.Publish(new FinishedLoadNextLevelMessage(_currentStageData.CurrentRoomType));

            // Delay to wait for camera move to hero.
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: _cancellationTokenSource.Token, ignoreTimeScale: true);
            GameManager.Instance.SetGameStateType(GameStateType.GameplayLobby);
            await UniTask.Delay(TimeSpan.FromSeconds(0.25f), cancellationToken: _cancellationTokenSource.Token, ignoreTimeScale: true);
            // Fade Out
            AudioManager.Instance.PlaySfx(_heroAppear, 0.5f).Forget();
            SimpleMessenger.Publish(new FadeInMessage(0.5f, fadeTween, true, EntitiesManager.Instance.HeroData.Position, true));
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: _cancellationTokenSource.Token, ignoreTimeScale: true);
            // Update current Stage info.
            SetUpNewStage();

            SimpleMessenger.Publish(new EnteredNextLevelMessage(_currentStageData.CurrentRoomType));
        }

        private void SetUpNewStage()
        {
            _entityIntroduced = new();
            _entityWaitingToIntroducQueue = new();

            if (_currentStageLoadConfigItem.waveConfigs != null && _currentStageLoadConfigItem.waveConfigs.Length > 0)
            {
                GameManager.Instance.SetGameStateType(GameStateType.GameplayRunning);
                StartCurrentLevel();
            }
            else
            {
                SetUpAfterWinLevel();
            }
        }

        private async UniTask LoadLevelAsync(GameplayRoomType roomType)
        {
            // Destroy current level

            _isLoadedShopItems = false;

            foreach (var checkEndStage in _checkEndStageConditions)
                Destroy(checkEndStage.gameObject);

            _checkEndStageConditions = new();

            if (_currentLevelMap)
            {
                PoolManager.Instance.Return(_currentLevelMap.gameObject);
                _currentLevelMap = null;
            }

            await UniTask.Delay(TimeSpan.FromSeconds(0.1f), cancellationToken: _cancellationTokenSource.Token, ignoreTimeScale: true);

            // Load Level Graphic
            var gameBalancingConfig = GameBalancingConfig;
            var heroPoint = CalculateHeroPoint();
            var (newLevelMap, stageLoadConfig, updatedRoomType, gateSetupType) = await gameBalancingConfig.GetNextStage(heroPoint, roomType, _currentStageData);

            Debug.LogWarning($"room current: {updatedRoomType} - {gateSetupType}");
            _currentStageData.UpdateCurrentStage(updatedRoomType, gateSetupType);
            _currentStageLoadConfigItem = stageLoadConfig;
            var mapGameObject = await PoolManager.Instance.Rent(newLevelMap.prefabName, token: _cancellationTokenSource.Token);
            _currentLevelMap = mapGameObject.GetComponent<MapLevel>();

            SetUpRoomGates(_currentLevelMap, gateSetupType);
            MapManager.Instance.LoadLevelMap(_currentLevelMap);
        }

        private void SetUpRoomGates(MapLevel mapLevel, GameplayGateSetupType gameplayGateSetupType)
        {
            switch (gameplayGateSetupType)
            {
                case GameplayGateSetupType.None:
                case GameplayGateSetupType.Normal:
                    mapLevel.gates[0].gameObject.SetActive(true);
                    mapLevel.gates[0].SetUp(GameplayRoomType.Normal);
                    mapLevel.gates[1].gameObject.SetActive(false);
                    break;
                case GameplayGateSetupType.Elite:
                    mapLevel.gates[0].gameObject.SetActive(true);
                    mapLevel.gates[0].SetUp(GameplayRoomType.EliteSpeed);
                    mapLevel.gates[1].gameObject.SetActive(true);
                    mapLevel.gates[1].SetUp(GameplayRoomType.ElitePower);
                    break;
                case GameplayGateSetupType.NormalAndShop:
                    mapLevel.gates[0].gameObject.SetActive(true);
                    mapLevel.gates[0].SetUp(GameplayRoomType.Normal);
                    mapLevel.gates[1].gameObject.SetActive(true);
                    mapLevel.gates[1].SetUp(GameplayRoomType.Shop);
                    break;
                case GameplayGateSetupType.Shop:
                    mapLevel.gates[0].gameObject.SetActive(true);
                    mapLevel.gates[0].SetUp(GameplayRoomType.Shop);
                    mapLevel.gates[1].gameObject.SetActive(false);
                    break;
                case GameplayGateSetupType.Boss:
                    mapLevel.gates[0].gameObject.SetActive(true);
                    mapLevel.gates[0].SetUp(GameplayRoomType.Boss);
                    mapLevel.gates[1].gameObject.SetActive(false);
                    break;
                default:
                    break;
            }
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
            var allShopItems = shopInGameManager.CurrentShopInGameItems;
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
                        // Finished time
                        HandleLoseStage();
                }
            }
            else
            {
                // Give player card for tutorial
                if(CurrentWaveIndex == 0 && _currentStageData.CurrentRoomType == GameplayRoomType.TutorialStage)
                {
                    ToastController.Instance.Show("Received auto gun skill!");
                    OnSelectBuffItemAsync(new ArtifactIdentity(ArtifactType.AutoStableGun, 0, 0)).Forget();

                    var tutorialManager = FindObjectOfType<TutorialManager>();
                    if (tutorialManager)
                        tutorialManager.SetText("Wait to collect rune on map!");
                }

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
            HandleLoseStage();
        }

        private void HandleLoseStage()
        {
            SimpleMessenger.Publish(new FinishedCurrentLevelMessage(false, true));
            ScreenNavigator.Instance.LoadSingleScreen(new WindowOptions(ScreenIds.LOSE), true).Forget();
        }

        private UniTask HandleWinLevelAsync()
        {
            if(_currentStageData.CurrentRoomType == GameplayRoomType.TutorialStage)
            {
                DataManager.Local.playerBasicLocalData.SetCompleteTut(TutorialType.GuideGameplay);
                DataManager.Local.SavePlayerData();
            }

            if(_currentStageData.StageNumber >= GameBalancingConfig.StageEndGame)
            {
                SimpleMessenger.Publish(new FinishedCurrentLevelMessage(true, true));
                EntitiesManager.Instance.HeroData.ReactionChangedEvent.Invoke(EntityReactionType.Win);
                ScreenNavigator.Instance.LoadSingleScreen(new WindowOptions(ScreenIds.VICTORY), true).Forget();
            }
            else
            {
                SimpleMessenger.Publish(new FinishedCurrentLevelMessage(true, false));
                SetUpAfterWinLevel();
                GiveRewards(RewardCoins);
            }
            return UniTask.CompletedTask;
        }

        private void SetUpAfterWinLevel()
        {
            GameManager.Instance.SetGameStateType(GameStateType.GameplayLobby);
            MechanicSystemManager.ResetForNextStage().Forget();
            var runes = FindObjectsByType<RuneArtifact>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            foreach (var item in runes)
            {
                PoolManager.Instance.Return(item.gameObject);
            }

            if (_currentStageData.CurrentRoomType == GameplayRoomType.EliteSpeed 
                || _currentStageData.CurrentRoomType == GameplayRoomType.ElitePower
                || _currentStageData.CurrentRoomType == GameplayRoomType.NormalHaveArtifact)
            {
                if (_currentStageData.CurrentRoomType == GameplayRoomType.ElitePower
                    || _currentStageData.CurrentRoomType == GameplayRoomType.EliteSpeed
                    )
                {
                    var shopChest = Instantiate(_shopChest);
                    _checkEndStageConditions.Add(shopChest);
                    shopChest.transform.position = MapManager.Instance.SpawnPoints[1].transform.position;
                }
                else if (_currentStageData.CurrentRoomType == GameplayRoomType.NormalHaveArtifact)
                {
                    var artifactChest = Instantiate(_artifactChest);
                    _checkEndStageConditions.Add(artifactChest);
                    artifactChest.transform.position = MapManager.Instance.SpawnPoints[1].transform.position;
                }
            }
            else
            {
                OpenNextLevel();
            }
        }

        private void OpenNextLevel()
        {
            _isWinCurrentLevel = true;
            foreach (var gate in _currentLevelMap.gates)
                gate.OpenGate();
        }

        private void StartCurrentLevel()
        {
            _isWinCurrentLevel = false;
            CurrentWaveIndex = 0;
            waveTimer.SetUp();
            maxWaveIndex = _currentStageLoadConfigItem.waveConfigs.Max(x => x.waveIndex);
            StartWave();
        }

        private void OnSelectBuffItem(ArtifactIdentity dataIdentity)
        {
            OnSelectBuffItemAsync(dataIdentity).Forget();
        }

        private async UniTaskVoid OnSelectBuffItemAsync(ArtifactIdentity dataIdentity)
        {
            var heroData = EntitiesManager.Instance.HeroData;
            await mechanicSystemManager.AddArtifactystem((IEntityControlData)heroData, dataIdentity.artifactType, dataIdentity.dataId);
        }

        public void GiveRewards(int numberCoins)
        {
            ToastController.Instance.Show($"Add + {numberCoins}");
            DataManager.Transient.AddMoney(InGameMoneyType.Gold, numberCoins);
        }

        public void AddIntroduced(int entityId)
        {
            _entityIntroduced.Add(entityId);
        }

        public void AddIntroduceToQueue(EntityIntroducedMessage message)
        {
            _entityWaitingToIntroducQueue.Enqueue(message);
        }

        public (EntityIntroducedMessage, bool) PopIntroduceInQueue()
        {
            var result = _entityWaitingToIntroducQueue.TryDequeue(out var message);
            return (message, result);
        }

        #endregion Class Methods
    }

}