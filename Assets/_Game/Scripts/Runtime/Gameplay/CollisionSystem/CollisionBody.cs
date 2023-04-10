using UnityEngine;
using Runtime.Gameplay.CollisionDetection;

namespace Runtime.Gameplay.EntitySystem
{
    [RequireComponent(typeof(Collider2D))]
    public class CollisionBody : Disposable, ICollisionBody
    {
        #region Members

        [SerializeField]
        private CollisionSearchTargetType _collisionBodySearchType = CollisionSearchTargetType.All;
        private ICollisionShape _collisionShape;
        private Collider2D _collider;

        #endregion Members

        #region Properties

        public int RefId { get; set; }
        public ICollisionShape CollisionShape => _collisionShape;
        public CollisionSearchTargetType CollisionSearchTargetType => _collisionBodySearchType;
        public Vector2 CollisionSystemPosition => _collider.bounds.center;
        public Collider2D Collider => _collider;
        public CollisionBodyType CollisionBodyType => CollisionBodyType.Default;

        #endregion Properties

        #region API Methods

        private void OnEnable()
        {
            _collider = gameObject.GetComponent<Collider2D>();
            _collisionShape = this.CreateCollisionShape(_collider);
            HasDisposed = false;
            CollisionSystem.Instance.AddBody(this);
        }

        private void OnDisable()
            => Dispose();

        #endregion API Methods

        #region Class Methods

        public void OnCollision(CollisionResult result, ICollisionBody other) { }

        public override void Dispose()
        {
            if (!HasDisposed)
            {
                HasDisposed = true;
                CollisionSystem.Instance.RemoveBody(this);
            }
        }

        #endregion Class Methods
    }
}