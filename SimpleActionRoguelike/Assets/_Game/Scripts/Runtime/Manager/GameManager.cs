using Cysharp.Threading.Tasks;
using Runtime.Constants;
using Runtime.Core.Singleton;
using Runtime.Definition;
using Runtime.Localization;
using Runtime.Manager.Audio;
using Runtime.Manager.Data;
using Runtime.Manager.Gameplay;
using Runtime.UI;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ZBase.UnityScreenNavigator.Core.Views;

namespace Runtime.Manager
{
    public class GameManager : MonoSingleton<GameManager>
    {
        [SerializeField] private bool _isTest;

        [SerializeField] private GameStateType[] _pausedGameStates;

        private GameStateType _currentGameStateType;
        private GameStateType _previousGameStateType;

        public bool IsTest => _isTest;

        public GameStateType CurrentGameStateType
        {
            get => _currentGameStateType;
        }

        protected override void Awake()
        {
            base.Awake();
            _currentGameStateType = GameStateType.None;
            InitializeAsync().Forget();
        }

        private async UniTaskVoid InitializeAsync()
        {
            AudioManager.Instance.PlayMusic(MusicIds.START_SCREEN, 1.5f);

            await LocalizeManager.InitializeAsync();
        }

        public bool SetGameStateType(GameStateType gameStateType, bool savePreviousGameState = false)
        {
            var currentGameState = _currentGameStateType;
            if (savePreviousGameState)
            {
                _previousGameStateType = currentGameState;
            }

            _currentGameStateType = gameStateType;
            if (_pausedGameStates.Contains(_currentGameStateType))
                Time.timeScale = 0;
            else
                Time.timeScale = 1;

            Debug.LogWarning(_currentGameStateType);

            return currentGameState != gameStateType;
        }

        public void ReturnPreviousGameState()
        {
            _currentGameStateType = _previousGameStateType;
            if (_pausedGameStates.Contains(_currentGameStateType))
                Time.timeScale = 0;
            else
                Time.timeScale = 1;

            Debug.LogWarning(_currentGameStateType);
        }

        public async UniTask StartLoadingGameplayAsync()
        {
            if (CurrentGameStateType == GameStateType.Loading)
                return;

            var loadingLayer = FindObjectOfType<LoadingLayer>();
            SetGameStateType(GameStateType.Loading);
            if (loadingLayer)
                await loadingLayer.StartLoading();

            await GameplayDataManager.Instance.InitAsync();
            await ScreenNavigator.Instance.LoadSingleScreen(new WindowOptions(ScreenIds.GAMEPLAY), true);
            await GameplayManager.Instance.InitAsync();

            if (loadingLayer)
                await loadingLayer.EndLoading();

            AudioManager.Instance.PlayMusic(MusicIds.MUSIC_GAMEPLAY);
        }

        public async UniTask GoToLobbyScreen()
        {
            AudioManager.Instance.PlayMusic(MusicIds.LOBBY_SCREEN, 1.5f);
            var windowOptions = new WindowOptions(ScreenIds.LOBBY);
            await ScreenNavigator.Instance.LoadSingleScreen(windowOptions, true);
        }

        public async UniTask BackHomeAsync()
        {
            if (CurrentGameStateType == GameStateType.Loading)
                return;

            GameplayDataManager.Instance.Dispose();
            GameplayManager.Instance.Dispose();

            var loadingLayer = FindObjectOfType<LoadingLayer>();
            SetGameStateType(GameStateType.Loading);
            if (loadingLayer)
                await loadingLayer.StartLoading();

            AudioManager.Instance.PlayMusic(MusicIds.LOBBY_SCREEN, 1.5f);
            await ScreenNavigator.Instance.LoadSingleScreen(new WindowOptions(ScreenIds.LOBBY), true);

            if (loadingLayer)
                await loadingLayer.EndLoading();
        }

        public async UniTask Replay()
        {
            if (CurrentGameStateType == GameStateType.Loading)
                return;

            // Replay
            GameplayDataManager.Instance.Dispose();
            GameplayManager.Instance.Dispose();
            await StartLoadingGameplayAsync();
        }

        [Button("Clear All Data")]
        public void ClearAllData()
        {
            DataManager.Local.ClearAllData();
        }
    }
}