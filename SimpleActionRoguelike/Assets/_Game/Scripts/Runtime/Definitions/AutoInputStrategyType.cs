namespace Runtime.Definition
{
    public enum AutoInputStrategyType
    {
        KeepDistanceToTarget = 0,
        MoveTowardTarget = 1,
        MoveRandomAroundTarget = 2,
        MoveByWay = 3, // Horizontal or vertical
        Idle = 4,
        MoveOnPreDefinedPath = 5,
        MoveOnPreDefinedPathFollowTarget = 6,

        FlyToHero = 7,
        FlyRandomAroundHero = 8,
    }
}