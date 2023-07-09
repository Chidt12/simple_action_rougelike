namespace Runtime.Definition
{
    public enum ArtifactType // Có những item sẽ được spawn ngẫu nhiên ở trên map
    {
        None = 0,
        RotateOrbs = 1, // trigger will attack nearly target, giết 1 quái thì được hồi 1 cầu, trigger thì bắn cầu đi => item sẽ có thử tự ưu tiên trigger ví dụ như quả cầu này sẽ luôn là least.
        UpgradeWeapon = 2,
        AutoStableGun = 3,
        StatusForwardArea = 4,
        StatusStorm = 5,
        StatusAroundArea = 6,
        BuffAllStats = 7, // nhặt and trigger => done visual
        CreateBuffFlag = 8,
        AutoRobot = 9, // **
        Boomerang = 10, // **
    }
}
