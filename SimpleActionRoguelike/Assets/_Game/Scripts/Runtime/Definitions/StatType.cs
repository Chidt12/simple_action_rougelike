namespace Runtime.Definition
{
    public static class StatTypeExtensions
    {
        public static bool IsPercentValue(this StatType statType)
        {
            switch (statType)
            {
                case StatType.Health:
                case StatType.Armor:
                case StatType.MoveSpeed:
                case StatType.AttackSpeed:
                case StatType.AttackDamage:
                case StatType.DetectRange:
                case StatType.CollideDamage:
                    return false;

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


        // Stat use for mechanics
        OrbBonusRotateSpeed = 100,

    }
}