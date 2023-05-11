namespace Runtime.Gameplay.CollisionDetection
{
    public enum CollisionShapeType
    {
        Rectangle,
        Circle,
        Collider
    }

    public interface ICollisionShape
    {
        #region Properties

        public ICollisionBody Parent { get; }
        public float ExtentX { get; }
        public float ExtentY { get; }
        public double MaxExtent { get; }

        #endregion Properties

        #region Interface Methods

        public CollisionResult TestCollision(ICollisionShape other);
        public CollisionShapeType CollisionShapeType { get; }
        void DrawGizmos();

        #endregion Interface Methods
    }
}