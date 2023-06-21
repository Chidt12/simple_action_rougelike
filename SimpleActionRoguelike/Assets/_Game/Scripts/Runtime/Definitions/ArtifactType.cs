namespace Runtime.Definition
{
    public enum ArtifactType // Có những item sẽ được spawn ngẫu nhiên ở trên map
    {
        None = 0,
        RotateOrbs = 1, // trigger will attack nearly target, giết 1 quái thì được hồi 1 cầu, trigger thì bắn cầu đi => item sẽ có thử tự ưu tiên trigger ví dụ như quả cầu này sẽ luôn là least.
        UpgradeWeapon = 2, // upgrade ưeapon
        AutoStableGun = 3, // nhặt and trigger
        FreezeArea = 4, // nhặt and trigger
        LightningStorm = 5, // nhặt and trigger
        BuffAllStats = 6, // nhặt and trigger
        CreateBuffFlag = 7, // nhặt and trigger
        AutoRobot = 8, // nhặt and trigger
        Boomerang = 9, // nhặt and trigger
    }
}
