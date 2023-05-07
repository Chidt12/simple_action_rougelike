namespace Runtime.Definition
{
    public enum StatusType
    {
        Knockback = 0,
        Pull = 1,

        Stun = 100,
        Freeze = 101,
        Root = 102,

        Taunt = 201,
        Terror = 202,

        Bleed = 301,
        Regen = 302,
        Dread = 303,
        Haste = 304,
        Quick = 305,
        Tough = 306,
        Hardened = 307,
        Chill = 308,
        Poison = 309,
        Fire = 310,

        None = 10000,
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
