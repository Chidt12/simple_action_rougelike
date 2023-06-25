namespace Runtime.Definition
{
    public enum EntityType
    {
        Hero,
        Enemy,
        Asset,
        Trap,
        Boss,
        Obstacle
    }

    public enum EntityFormType // Change the visual and stuff;
    {
        Normal = 0,
        Form1 = 1,
        Form2 = 2
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