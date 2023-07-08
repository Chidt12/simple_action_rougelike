namespace Runtime.Definition
{
    public enum SkillType
    {
        None = 0,
        Shooting = 1,
        FireProjectileAround = 2,
        JumpAhead = 3,
        HitAttack = 4, // Các đòn đánh cơ bản => chém, đập => tạo box damage bình thường.
        RushAttack = 5,
        RotateAttack = 6, // **
        Summon = 7, // do because it easy
        Heal = 8, // **
        Protect = 9, // **
        BigJump = 10,
        SpawnLazer = 11, // **
        FireRoundProjectile = 12,
        ThrowBomb = 13
    }
}