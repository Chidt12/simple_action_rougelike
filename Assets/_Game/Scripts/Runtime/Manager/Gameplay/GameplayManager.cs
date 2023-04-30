using Cysharp.Threading.Tasks;
using Runtime.ConfigModel;
using Runtime.Core.Message;
using Runtime.Core.Singleton;
using Runtime.Gameplay;
using Runtime.Gameplay.EntitySystem;
using Runtime.Manager.Data;
using Runtime.Message;
using System.Linq;
using UnityEngine;
using ZBase.Foundation.PubSub;

namespace Runtime.Manager.Gameplay
{
    public class GameplayManager : MonoSingleton<GameplayManager>
    {
        protected WaveTimer waveTimer;
        protected bool hasFinishedSpawnWave;
        protected int maxWaveIndex;
        private ISubscription _gameplayDataLoadedRegistry;
        private ISubscription _entityDiedRegistry;

        private StageLoadConfigItem StageInfo => ConfigDataManager.Instance.GetStageConfigData(GameplayDataDispatcher.Instance.StageId);
        public int CurrentGameplayTimeInSecond => waveTimer.CurrentGameplayTime;
        public int CurrentWaveIndex { get; protected set; }

        #region API Methods

        protected override void Awake()
        {
            base.Awake();
            waveTimer = new WaveTimer();
            _gameplayDataLoadedRegistry = SimpleMessenger.Subscribe<GameplayDataLoadedMessage>(OnGameplayDataLoaded);
            _entityDiedRegistry = SimpleMessenger.Subscribe<EntityDiedMessage>(OnEntityDied);
        }

        #endregion API Methods

        #region Class Methods

        private void OnGameplayDataLoaded(GameplayDataLoadedMessage gameplayDataLoadedMessage)
        {
            EntitiesManager.Instance.Initialize();
            StartGameplay();
        }

        private void OnEntityDied(EntityDiedMessage entityDiedMessage)
        {
            var handleCharacterDiedResult = EntitiesManager.Instance.HandleCharacterDied(entityDiedMessage);
            if (handleCharacterDiedResult == HandleCharacterDiedResultType.DeletedAllEnemyOnMap)
                DeletedAllEnemyOnMap();
            else if (handleCharacterDiedResult == HandleCharacterDiedResultType.HeroDied)
                KillHero();
        }

        private void StartGameplay()
        {
            var stageInfo = StageInfo;
            waveTimer.SetUp();
            maxWaveIndex = stageInfo.waveConfigs.Max(x => x.waveIndex);
            StartWave();
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
        {

        }

        protected virtual void FinishWave(bool isClearWave)
        {
            var highestWaveIndex = maxWaveIndex;
            if (CurrentWaveIndex == highestWaveIndex)
            {
                if (isClearWave)
                {
                    HandleWinStage();
                }
                else
                {
                    var hasNoEnemiesLeft = EntitiesManager.Instance.HaveNoEnemiesLeft;
                    if (hasNoEnemiesLeft)
                        HandleWinStage();
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
            Debug.LogError("END GAME");
        }

        private void HandleWinStage()
        {
            Debug.LogError("WIN GAME");
        }

        private void HandleLoseStage()
        {
            Debug.LogError("LOSE GAME");
        }

        public void Dispose()
        {
            _gameplayDataLoadedRegistry.Dispose();
            _entityDiedRegistry.Dispose();
        }

        #endregion Class Methods
    }

}