using Runtime.Definition;
using System.Collections.Generic;
using System.Linq;

namespace Runtime.Gameplay.Manager
{
    public class CurrentLoadedStageData
    {
        private int _stageNumber;
        private int _countToBossStage;
        private List<GameplayRoomType> _gameplayRoomTypes;
        private GameplayRoomType _currentRoomType;

        public int StageNumber => _stageNumber;
        public int CountToBossStage => _countToBossStage;
        public GameplayRoomType CurrentRoomType => _currentRoomType;

        public CurrentLoadedStageData()
        {
            _stageNumber = 0;
            _countToBossStage = 0;
            _gameplayRoomTypes = new();
        }

        public void UpdateCurrentStage(GameplayRoomType roomType)
        {
            if(roomType == GameplayRoomType.Boss)
                _countToBossStage = 0;
            else
                _countToBossStage++;

            _stageNumber++;
            _currentRoomType = roomType;
            _gameplayRoomTypes.Add(roomType);
        }

        public int GetNumberRoomPassed(GameplayRoomType roomType)
        {
            return _gameplayRoomTypes.Count(x => x == roomType);
        }
    }
}
