namespace Runtime.Definition
{
    public enum GameplayRoomType
    {
        None = 0,
        Normal = 1,
        Elite = 2,
        Shop = 3,
        Boss = 4,

        EliteHaveArtifact = 5,
        NormalHaveArtifact = 6,
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
