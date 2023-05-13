using Runtime.Core.Singleton;
using Runtime.Definition;
using UnityEngine;

namespace Runtime.Manager
{
    public class GameManager : PersistentMonoSingleton<GameManager>
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
    }
}