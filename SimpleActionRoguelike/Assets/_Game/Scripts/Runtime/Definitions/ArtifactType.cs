namespace Runtime.Definition
{
    public enum ArtifactType // Có những item sẽ được spawn ngẫu nhiên ở trên map
    {
        None = 0,
        RotateOrbs = 1, // trigger will attack nearly target, giết 1 quái thì được hồi 1 cầu, trigger thì bắn cầu đi => item sẽ có thử tự ưu tiên trigger ví dụ như quả cầu này sẽ luôn là least.
        UpgradeWeapon = 2, // upgrade ưeapon
        AutoStableGun = 3, // nhặt and trigger
        StatusForwardArea = 4, // nhặt and trigger
        StatusStorm = 5, // nhặt and trigger
        StatusAroundArea = 6, // nhặt and trigger
        BuffAllStats = 7, // nhặt and trigger
        CreateBuffFlag = 8, // nhặt and trigger
        AutoRobot = 9, // nhặt and trigger
        Boomerang = 10, // nhặt and trigger
    }
}
