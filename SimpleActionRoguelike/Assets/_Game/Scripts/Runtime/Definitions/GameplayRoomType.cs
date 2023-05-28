namespace Runtime.Definition
{
    public enum GameplayRoomType
    {
        Normal = 0,
        Elite = 1,
        Shop = 2,
        Boss = 3,

        EliteHaveArtifact = 4,
        NormalHaveArtifact = 5,
        Lobby = 1_000,
    }

    public enum GameplayGateSetupType
    {
        None = 0,
        Normal = 1,
        NormalAndElite = 2,
        NormalAndShop = 3,
        Shop = 4,
        Boss = 5,
    }
}
