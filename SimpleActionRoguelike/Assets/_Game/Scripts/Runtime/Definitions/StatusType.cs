namespace Runtime.Definition
{
    public enum StatusType
    {
        None = 0,
        Knockback = 1000,
        Pull = 1001,

        Stun = 900,
        Freeze = 901,
        Root = 902,

        Taunt = 801,
        Terror = 802,

        Bleed = 701,
        Regen = 702,
        Dread = 703,
        Haste = 704,
        Quick = 705,
        Tough = 706,
        Hardened = 707,
        Chill = 708,
        Poison = 709,
        Fire = 710,
    }

    public static class EntityStatusStateExtensions
    {
        public static bool IsInMovementLockedStatus(this StatusType entityStatusState)
        {
            switch (entityStatusState)
            {
                case StatusType.Stun:
                case StatusType.Knockback:
                case StatusType.Pull:
                case StatusType.Freeze:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsInSkillLockedStatus(this StatusType entityStatusState)
        {
            switch (entityStatusState)
            {
                case StatusType.Stun:
                case StatusType.Knockback:
                case StatusType.Pull:
                case StatusType.Freeze:
                case StatusType.Terror:
                case StatusType.Root:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsInAttackLockedStatus(this StatusType entityStatusState)
        {
            switch (entityStatusState)
            {
                case StatusType.Stun:
                case StatusType.Knockback:
                case StatusType.Pull:
                case StatusType.Freeze:
                case StatusType.Terror:
                case StatusType.Root:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsInHardCCStatus(this StatusType entityStatusState)
        {
            switch (entityStatusState)
            {
                case StatusType.Stun:
                case StatusType.Knockback:
                case StatusType.Pull:
                case StatusType.Freeze:
                case StatusType.Terror:
                case StatusType.Taunt:
                case StatusType.Root:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsInHardCCPriority2Status(this StatusType entityStatusState)
        {
            switch (entityStatusState)
            {
                case StatusType.Stun:
                case StatusType.Freeze:
                case StatusType.Root:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsInHardCCPriority3Status(this StatusType entityStatusState)
        {
            switch (entityStatusState)
            {
                case StatusType.Knockback:
                case StatusType.Pull:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsInAnimationLockedStatus(this StatusType entityStatusState)
        {
            switch (entityStatusState)
            {
                case StatusType.Freeze:
                    return true;
                default:
                    return false;
            }
        }
    }
}
