using Cysharp.Threading.Tasks;
using Runtime.Constants;
using Runtime.Core.Singleton;
using Runtime.Definition;
using Runtime.Gameplay;
using Runtime.Manager.Gameplay;
using Runtime.UI;
using UnityEngine;
using ZBase.UnityScreenNavigator.Core.Views;

namespace Runtime.Manager
{
    public class GameManager : MonoSingleton<GameManager>
    {
        private GameStateType _currentGameStateType;
        private GameStateType _previousGameStateType;
        public GameStateType CurrentGameStateType
        {
            get => _currentGameStateType;

            set
            {
                _previousGameStateType = _currentGameStateType;
                _currentGameStateType = value;

                if (_currentGameStateType == GameStateType.GameplayPausing)
                    Time.timeScale = 0;
                else
                    Time.timeScale = 1;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            _currentGameStateType = GameStateType.None;
            _previousGameStateType = GameStateType.None;
        }

        public bool SetGameStateType(GameStateType gameStateType)
        {
            var currentGameState = CurrentGameStateType;
            CurrentGameStateType = gameStateType;
            return currentGameState != gameStateType;
        }

        public bool ReturnPreviousGameStateType() 
        {
            var currentGameState = CurrentGameStateType;
            CurrentGameStateType = _previousGameStateType;
            _previousGameStateType = currentGameState;
            return true;
        }

        public async UniTaskVoid StartLoadingGameplayAsync()
        {
            if (CurrentGameStateType == GameStateType.Loading)
                return;

            var loadingLayer = FindObjectOfType<LoadingLayer>();
            SetGameStateType(GameStateType.Loading);
            if (loadingLayer)
                await loadingLayer.StartLoading();

            await GameplayDataManager.Instance.InitAsync();
            await ScreenNavigator.Instance.LoadSingleScreen(new WindowOptions(ScreenIds.GAMEPLAY));
            await GameplayManager.Instance.InitAsync();

            if (loadingLayer)
                await loadingLayer.EndLoading();
            SetGameStateType(GameStateType.GameplayRunning);
        }



        public void Replay()
        {
            // Replay
        }
    }
}