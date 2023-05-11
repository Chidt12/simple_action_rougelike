namespace Runtime.Definition
{
    public enum EntityType
    {
        Hero,
        Enemy,
        Object,
        Trap,
        Boss,
        Obstacle
    }

    public static class EntityTypeExtensions
    {
        public static bool IsDisplayWarningExecuteSkill(this EntityType entityType)
        {
            switch (entityType)
            {
                case EntityType.Hero:
                    return false;
                default:
                    return true;
            }
        }
    }
}