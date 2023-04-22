namespace Runtime.Definition
{
    public static class StatModifyTypeExtension
    {
        public static bool IsPercentValue(this StatModifyType statModifyType)
        {
            switch (statModifyType)
            {
                case StatModifyType.BaseBonus:
                case StatModifyType.TotalBonus:
                    return false;
                case StatModifyType.BaseMultiply:
                case StatModifyType.TotalMultiply:
                    return true;
                default:
                    return false;
            }
        }
    }

    public enum StatModifyType
    {
        BaseBonus = 0,
        BaseMultiply = 1,
        TotalBonus = 2,
        TotalMultiply = 3
    }
}