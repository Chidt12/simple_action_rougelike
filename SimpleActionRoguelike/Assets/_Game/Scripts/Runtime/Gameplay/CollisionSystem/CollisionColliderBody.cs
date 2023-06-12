using UnityEngine;
using Runtime.Gameplay.CollisionDetection;
using Runtime.Manager.Gameplay;

namespace Runtime.Gameplay.EntitySystem
{
    [RequireComponent(typeof(Collider2D))]
    public class CollisionColliderBody : Disposable, ICollisionBody
    {
        #region Members

        [SerializeField] private CollisionSearchTargetType[] _collisionBodySearchTypes;
        [SerializeField] private CollisionBodyType _collisionBodyType = CollisionBodyType.Default;
        [SerializeField] private bool _shouldUpdateMap;
        private ICollisionShape _collisionShape;
        private Collider2D _collider;

        #endregion Members

        #region Properties

        public int RefId { get; set; }
        public ICollisionShape CollisionShape => _collisionShape;
        public Vector2 CollisionSystemPosition => _collider.bounds.center;
        public Collider2D Collider => _collider;
        public CollisionBodyType CollisionBodyType => _collisionBodyType;
        public CollisionSearchTargetType[] CollisionSearchTargetTypes => _collisionBodySearchTypes;
        public float MaxBoundSize => Mathf.Ceil(Mathf.Max(_collider.bounds.extents.x * 2, _collider.bounds.extents.y * 2));

        #endregion Properties

        #region API Methods

        private void OnEnable()
        {
            _collider = gameObject.GetComponent<Collider2D>();
            _collisionShape = this.CreateCollisionShape(_collider);
            HasDisposed = false;
            CollisionSystem.Instance.AddBody(this);
            if(_shouldUpdateMap)
                MapManager.Instance.UpdateMapWithAroundPoints(transform.position, MaxBoundSize);
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
                if (_shouldUpdateMap)
                    MapManager.Instance.UpdateMapWithAroundPoints(transform.position, MaxBoundSize);
            }
        }

        #endregion Class Methods
    }
}