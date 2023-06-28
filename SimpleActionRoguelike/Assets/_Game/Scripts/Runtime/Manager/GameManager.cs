using Cysharp.Threading.Tasks;
using Runtime.Constants;
using Runtime.Core.Singleton;
using Runtime.Definition;
using Runtime.Localization;
using Runtime.Manager.Gameplay;
using Runtime.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZBase.UnityScreenNavigator.Core.Views;

namespace Runtime.Manager
{
    public class GameManager : MonoSingleton<GameManager>
    {
        [SerializeField] private bool _isTest;

        private GameStateType _currentGameStateType;
        private Stack<GameStateType> _previousGameStateTypeStack;

        public bool IsTest => _isTest;

        public GameStateType CurrentGameStateType
        {
            get => _currentGameStateType;
        }

        protected override void Awake()
        {
            base.Awake();
            _currentGameStateType = GameStateType.None;
            _previousGameStateTypeStack = new();
            InitializeAsync().Forget();
        }

        private async UniTaskVoid InitializeAsync()
        {
            await LocalizeManager.InitializeAsync();
        }

        public void ClearGameStateHistory()
        {
            _previousGameStateTypeStack.Clear();
        }

        public bool SetGameStateType(GameStateType gameStateType)
        {
            var currentGameState = _currentGameStateType;
            _previousGameStateTypeStack.Push(_currentGameStateType);
            _currentGameStateType = gameStateType;

            if (_currentGameStateType == GameStateType.GameplayPausing || _currentGameStateType == GameStateType.GameplayChoosingItem)
                Time.timeScale = 0;
            else
                Time.timeScale = 1;

            return currentGameState != gameStateType;
        }

        public bool ReturnPreviousGameStateType() 
        {
            if(_previousGameStateTypeStack.Count > 0)
            {
                _currentGameStateType = _previousGameStateTypeStack.Pop();
                if (_currentGameStateType == GameStateType.GameplayPausing || _currentGameStateType == GameStateType.GameplayChoosingItem)
                    Time.timeScale = 0;
                else
                    Time.timeScale = 1;

                return true;
            }
            return false;
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

            SetGameStateType(GameStateType.GameplayRunning);
            ClearGameStateHistory();
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
            await ScreenNavigator.Instance.LoadSingleScreen(new WindowOptions(ScreenIds.LOBBY), true);

            if (loadingLayer)
                await loadingLayer.EndLoading();
            ClearGameStateHistory();
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
    }
}