namespace Runtime.Gameplay.Manager
{
    public class CurrentLoadedStageData
    {
        private int _stageNumber;
        private int _enteredShopRoom;
        private int _ownedShopItemNumber;
        private int _ownedArtifactNumber;

        public int EnteredShopRoom => _enteredShopRoom;

        public int OwnedArtifactNumber => _ownedArtifactNumber;
        public int OwnedShopItemNumber => _ownedShopItemNumber;
        public int StageNumber => _stageNumber;
    }
}
