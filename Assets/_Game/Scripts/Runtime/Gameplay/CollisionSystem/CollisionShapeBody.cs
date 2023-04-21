using Runtime.Gameplay.CollisionDetection;
using System;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    [RequireComponent(typeof(Collider2D))]
    public abstract class CollisionShapeBody : Disposable, ICollisionBody
    {
        #region Members

        [SerializeField]
        private CollisionSearchTargetType _collisionBodySearchType = CollisionSearchTargetType.All;
        [SerializeField]
        private CollisionBodyType _collisionBodyType = CollisionBodyType.Default;
        private ICollisionShape _collisionShape;

        #endregion Members

        #region Properties

        public int RefId { get; set; }
        public ICollisionShape CollisionShape => _collisionShape;
        public CollisionSearchTargetType CollisionSearchTargetType => _collisionBodySearchType;
        public Vector2 CollisionSystemPosition => transform.position;
        public Collider2D Collider => null;
        public CollisionBodyType CollisionBodyType => _collisionBodyType;

        protected Action<CollisionResult, ICollisionBody> OnCollisionEvent { get; set;}

        #endregion Properties

        #region API Methods

        public void Init()
        {
            _collisionShape = CreateShape();
            HasDisposed = false;
            CollisionSystem.Instance.AddBody(this);
        }

        private void OnDisable()
            => Dispose();

        #endregion API Methods

        #region Class Methods

        protected abstract ICollisionShape CreateShape();

        public void OnCollision(CollisionResult result, ICollisionBody other) => OnCollisionEvent?.Invoke(result, other);

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