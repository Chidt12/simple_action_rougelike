using Runtime.Definition;
using System.Collections.Generic;
using System.Linq;

namespace Runtime.Gameplay.Manager
{
    public class CurrentLoadedStageData
    {
        private int _stageNumber;
        private int _countToBossStage;
        private List<GameplayGateSetupType> _gameplayGateSetupTypesBeforeBoss;
        private List<GameplayRoomType> _gameplayRoomTypesBeforeBoss;
        private GameplayRoomType _currentRoomType;

        public int StageNumber => _stageNumber;
        public int CountToBossStage => _countToBossStage;
        public GameplayRoomType CurrentRoomType => _currentRoomType;

        public CurrentLoadedStageData()
        {
            _stageNumber = 0;
            _countToBossStage = 0;
            _gameplayGateSetupTypesBeforeBoss = new();
            _gameplayRoomTypesBeforeBoss = new();
        }

        public void UpdateCurrentStage(GameplayRoomType roomType, GameplayGateSetupType gateSetUpType)
        {
            if(roomType == GameplayRoomType.Boss)
            {
                _countToBossStage = 0;
                _gameplayGateSetupTypesBeforeBoss.Clear();
                _gameplayRoomTypesBeforeBoss.Clear();
            }
            else
            {
                _countToBossStage++;
                _gameplayRoomTypesBeforeBoss.Add(roomType);
            }

            _gameplayGateSetupTypesBeforeBoss.Add(gateSetUpType);
            _stageNumber++;
            _currentRoomType = roomType;
        }

        public int GetNumberRoomPassed(GameplayRoomType roomType)
        {
            return _gameplayRoomTypesBeforeBoss.Count(x => x == roomType);
        }

        public int GetNumberOfGateSetUpPassed(GameplayGateSetupType gateSetUpType)
        {
            return _gameplayGateSetupTypesBeforeBoss.Count(x => x == gateSetUpType);
        }
    }
}
