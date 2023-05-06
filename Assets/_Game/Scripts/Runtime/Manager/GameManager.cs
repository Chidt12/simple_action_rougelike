using Runtime.Core.Singleton;
using Runtime.Definition;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Manager
{
    public class GameManager : PersistentMonoSingleton<GameManager>
    {
        private GameStateType _currentGameStateType;
        private GameStateType _previousGameStateType;
        public GameStateType CurrentGameStateType { get;}

        protected override void Awake()
        {
            base.Awake();
            _currentGameStateType = GameStateType.None;
            _previousGameStateType = GameStateType.None;
        }

        public bool SetGameStateType(GameStateType gameStateType)
        {
            _previousGameStateType = _currentGameStateType;
            _currentGameStateType = gameStateType;
            return true;
        }
    }
}