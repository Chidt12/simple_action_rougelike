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
using Runtime.Gameplay.Manager;
using Runtime.Helper;
using Runtime.Manager.Data;
using Runtime.Message;
using Runtime.SceneLoading;
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

        protected int RewardCoins = 2;

        protected WaveTimer waveTimer;
        protected bool hasFinishedSpawnWave;
        protected int maxWaveIndex;
        private ISubscription _gameplayDataLoadedRegistry;
        private ISubscription _entityDiedRegistry;
        private ISubscription _goToNextLevelMapRegistry;
        private CancellationTokenSource _cancellationTokenSource;
        private bool _isWinCurrentLevel;

        private MapLevel _currentLevelMap;
        private StageLoadConfigItem _currentStageLoadConfigItem;
        private CurrentLoadedStageData _currentStageData;
        private List<CheckEndStage> _checkEndStageConditions;

        public GameBalancingConfig GameBalancingConfig => GameplayDataManager.Instance.GetGameBalancingConfig();
        public int CurrentGameplayTimeInSecond => waveTimer.CurrentGameplayTime;
        public int CurrentWaveIndex { get; protected set; }

        #region API Methods

        protected override void Awake()
        {
            base.Awake();
            waveTimer = new WaveTimer();
            _checkEndStageConditions = new();
            _cancellationTokenSource = new CancellationTokenSource();
            _gameplayDataLoadedRegistry = SimpleMessenger.Subscribe<GameplayDataLoadedMessage>(OnGameplayDataLoaded);
            _entityDiedRegistry = SimpleMessenger.Subscribe<EntityDiedMessage>(OnEntityDied);
            _goToNextLevelMapRegistry = SimpleMessenger.Subscribe<SendToGameplayMessage>(OnReceiveMessage);
            SceneLoaderManager.RegisterScenePreloadedAction(Dispose);
        }

        #endregion API Methods

        #region Class Methods

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
            var shopItems = await ConfigDataManager.Instance.LoadCurrentSuitableShopInGameItems();
            var selectInGameShopData = new ModalGiveInGameShopData(shopItems.ToArray(), OnGiveShopItem);
            await ScreenNavigator.Instance.LoadModal(new WindowOptions(ModalIds.GIVE_INGAME_SHOP), selectInGameShopData);
        }

        public async UniTaskVoid LoadShopInGameAsync()
        {
            var shopItems = await ConfigDataManager.Instance.LoadCurrentSuitableShopInGameItems();
            var selectInGameShopData = new ModalSelectIngameShopData(shopItems.ToArray(), OnBuyShopItem);
            await ScreenNavigator.Instance.LoadModal(new WindowOptions(ModalIds.SELECT_INGAME_SHOP), selectInGameShopData);
        }

        private void OnBuyShopItem(ShopInGameStageLoadConfigItem item)
        {
            if (item.cost.resourceType == ResourceType.MoneyInGame)
            {
                var value = DataManager.Transient.GetGameMoneyType((InGameMoneyType)item.cost.resourceId);
                if (value < item.cost.resourceNumber)
                {
                    ToastController.Instance.Show("Not Enough Resource!");
                    return;
                }

                DataManager.Transient.RemoveMoney((InGameMoneyType)item.cost.resourceId, item.cost.resourceNumber);

                AddShopItemAsync(item.identity).Forget();
            }
        }

        private void OnGiveShopItem(ShopInGameStageLoadConfigItem item)
        {
            AddShopItemAsync(item.identity).Forget();
        }

        private async UniTask AddShopItemAsync(ShopInGameIdentity identity)
        {
            await ShopInGameManager.Instance.AddShopInGameItem(EntitiesManager.Instance.HeroData, identity.shopInGameItemType, identity.dataId);
            var dataConfig = await ConfigDataManager.Instance.LoadShopInGameDataConfig(identity.shopInGameItemType);
            var description = await dataConfig.GetDescription(identity.dataId);
            ToastController.Instance.Show(description);
        }

        private async UniTaskVoid LoadGiveArtifactAsync()
        {
            // Pause for select buff
            GameManager.Instance.SetGameStateType(GameStateType.GameplayPausing);

            var heroEntityData = EntitiesManager.Instance.HeroData;
            var currentBuffs = MechanicSystemManager.Instance.GetCurrentBuffsInGame();

            var suitableItems = await DataManager.Config.LoadCurrentSuitableBuffInGameItems(currentBuffs);

            var modalData = new ModalSelectIngameBuffData(heroEntityData, suitableItems.Select(x => x.identity).ToArray(), OnSelectBuffItem);
            ScreenNavigator.Instance.LoadModal(new WindowOptions(ModalIds.SELECT_INGAME_BUFF), modalData).Forget();
        }

        private async UniTaskVoid LoadNextLevelAsync(GameplayRoomType roomType)
        {
            GameManager.Instance.SetGameStateType(GameStateType.GameplayPausing);

            // Fade In
            var fadeTween = new TweenType(TweenHelper.TweenCurve.EaseInSinusoidal);
            SimpleMessenger.Publish(new FadeOutMessage(0.75f, fadeTween, true, EntitiesManager.Instance.HeroData.Position, false));
            await UniTask.Delay(TimeSpan.FromSeconds(0.75f), cancellationToken: _cancellationTokenSource.Token, ignoreTimeScale: true);

            // Load Level.
            await LoadLevelAsync(roomType);
            EntitiesManager.Instance.HeroData.ForceUpdatePosition.Invoke(MapManager.Instance.SpawnPoints[0].transform.position);

            // Delay to wait for camera move to hero.
            await UniTask.Delay(TimeSpan.FromSeconds(0.75f), cancellationToken: _cancellationTokenSource.Token, ignoreTimeScale: true);
            // Fade Out
            GameManager.Instance.ReturnPreviousGameStateType();
            SimpleMessenger.Publish(new FadeInMessage(0.5f, fadeTween, true, EntitiesManager.Instance.HeroData.Position, true));
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: _cancellationTokenSource.Token, ignoreTimeScale: true);

            // Update current Stage info.
            SetUpNewStage();
        }

        private async UniTaskVoid StartGameplay()
        {
            // Load Level
            _currentStageData = new();
            await LoadLevelAsync(GameplayRoomType.Lobby);

            // Load Hero.
            await EntitiesManager.Instance.CreateEntityAsync(
                new SpawnedEntityInfo(Constant.HERO_ID, EntityType.Hero, GameplayDataDispatcher.Instance.HeroLevel), 
                MapManager.Instance.SpawnPoints[0].transform.position,
                cancellationToken: _cancellationTokenSource.Token);

            SetUpNewStage();
        }

        private void SetUpNewStage()
        {
            if (_currentStageLoadConfigItem.waveConfigs != null && _currentStageLoadConfigItem.waveConfigs.Length > 0)
            {
                StartCurrentLevel();
            }
            else
            {
                WonCurrentLevel();
            }
        }

        private async UniTask LoadLevelAsync(GameplayRoomType roomType)
        {
            // Destroy current level

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
            CameraManager.Instance.SetConfinder(_currentLevelMap.confinder);
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
                case GameplayGateSetupType.NormalAndElite:
                    mapLevel.gates[0].gameObject.SetActive(true);
                    mapLevel.gates[0].SetUp(GameplayRoomType.Normal);
                    mapLevel.gates[1].gameObject.SetActive(true);
                    mapLevel.gates[1].SetUp(GameplayRoomType.Elite);
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

        private void HandleLoseStage()
        {
            Debug.LogError("LOSE GAME");
        }

        private UniTask HandleWinLevelAsync()
        {
            WonCurrentLevel();
            ToastController.Instance.Show($"Add + {RewardCoins}");
            DataManager.Transient.AddMoney(InGameMoneyType.Gold, RewardCoins);
            return UniTask.CompletedTask;
        }

        private void WonCurrentLevel()
        {
            if(_currentStageData.CurrentRoomType == GameplayRoomType.EliteHaveArtifact 
                || _currentStageData.CurrentRoomType == GameplayRoomType.Elite
                || _currentStageData.CurrentRoomType == GameplayRoomType.NormalHaveArtifact)
            {
                if (_currentStageData.CurrentRoomType == GameplayRoomType.Elite)
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
                else if (_currentStageData.CurrentRoomType == GameplayRoomType.EliteHaveArtifact)
                {
                    var shopChest = Instantiate(_shopChest);
                    _checkEndStageConditions.Add(shopChest);
                    shopChest.transform.position = MapManager.Instance.SpawnPoints[1].transform.position;
                    var artifactChest = Instantiate(_artifactChest);
                    _checkEndStageConditions.Add(artifactChest);
                    artifactChest.transform.position = MapManager.Instance.SpawnPoints[2].transform.position;
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
            foreach (var gate in _currentLevelMap.gates)
                gate.CloseGate();

            CurrentWaveIndex = 0;
            waveTimer.SetUp();
            maxWaveIndex = _currentStageLoadConfigItem.waveConfigs.Max(x => x.waveIndex);
            StartWave();
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