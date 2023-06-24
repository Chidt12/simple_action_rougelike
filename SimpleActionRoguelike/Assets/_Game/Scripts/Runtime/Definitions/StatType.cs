namespace Runtime.Definition
{
    public static class StatTypeExtensions
    {
        public static bool IsPercentValue(this StatType statType)
        {
            switch (statType)
            {
                case StatType.ArmorPenetration:
                case StatType.CritChance:
                case StatType.CritDamage:
                case StatType.LifeSteal:
                case StatType.DodgeChance:
                case StatType.CCReduction:
                case StatType.CooldownReduction:
                case StatType.DamageReduction:
                    return true;

                default:
                    return false;
            }
        }

        public static bool IsHaveCurrentValue(this StatType statType)
        {
            switch (statType)
            {
                case StatType.DashNumber:
                    return true;
                default:
                    return false;
            }
        }
    }

    public enum StatType
    {
        Health = 2,
        Armor = 3,
        ArmorPenetration = 4,
        CritChance = 5,
        CritDamage = 6,
        MoveSpeed = 7,
        LifeSteal = 8,
        AttackSpeed = 9,
        DodgeChance = 10,
        CCReduction = 11,
        CooldownReduction = 12,
        DamageReduction = 13,
        AttackDamage = 14,
        DetectRange = 15,
        CollideDamage = 16,
        Shield = 17,
        AttackRange = 18,

        // Stat use for mechanics
        OrbBonusRotateSpeed = 100,
        DashNumber = 101,
        ArtifactInterval = 102,
        RuneArtifactLifeTime = 103,
        RuneArtifactCooldown = 104
    }
}