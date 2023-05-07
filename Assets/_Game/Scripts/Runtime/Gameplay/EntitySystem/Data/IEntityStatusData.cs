namespace Runtime.Gameplay.EntitySystem
{
    public enum EntityStatusState
    {
        None = 0,
        Stunned = 1,
        Knockedback = 2,
        Pulled = 3,
        Terrored = 4,
        Taunted = 5,
        Freezed = 6,
        Rooted = 7
    }

    public static class EntityStatusStateExtensions
    {
        public static bool IsInMovementLockedStatus(this EntityStatusState entityStatusState)
        {
            switch (entityStatusState)
            {
                case EntityStatusState.Stunned:
                case EntityStatusState.Knockedback:
                case EntityStatusState.Pulled:
                case EntityStatusState.Freezed:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsInSkillLockedStatus(this EntityStatusState entityStatusState)
        {
            switch (entityStatusState)
            {
                case EntityStatusState.Stunned:
                case EntityStatusState.Knockedback:
                case EntityStatusState.Pulled:
                case EntityStatusState.Freezed:
                case EntityStatusState.Terrored:
                case EntityStatusState.Rooted:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsInAttackLockedStatus(this EntityStatusState entityStatusState)
        {
            switch (entityStatusState)
            {
                case EntityStatusState.Stunned:
                case EntityStatusState.Knockedback:
                case EntityStatusState.Pulled:
                case EntityStatusState.Freezed:
                case EntityStatusState.Terrored:
                case EntityStatusState.Rooted:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsInHardCCStatus(this EntityStatusState entityStatusState)
        {
            switch (entityStatusState)
            {
                case EntityStatusState.Stunned:
                case EntityStatusState.Knockedback:
                case EntityStatusState.Pulled:
                case EntityStatusState.Freezed:
                case EntityStatusState.Terrored:
                case EntityStatusState.Taunted:
                case EntityStatusState.Rooted:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsInHardCCPriority2Status(this EntityStatusState entityStatusState)
        {
            switch (entityStatusState)
            {
                case EntityStatusState.Stunned:
                case EntityStatusState.Freezed:
                case EntityStatusState.Rooted:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsInHardCCPriority3Status(this EntityStatusState entityStatusState)
        {
            switch (entityStatusState)
            {
                case EntityStatusState.Knockedback:
                case EntityStatusState.Pulled:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsInAnimationLockedStatus(this EntityStatusState entityStatusState)
        {
            switch (entityStatusState)
            {
                case EntityStatusState.Freezed:
                    return true;
                default:
                    return false;
            }
        }
    }

    public interface IEntityStatusData : IEntityData
    {
        public EntityStatusState CurrentState { get; }
    }
}