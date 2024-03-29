using Runtime.Gameplay.CollisionDetection;
using System;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public abstract class CollisionShapeBody : Disposable, ICollisionBody
    {
        #region Members

        [SerializeField]
        private CollisionSearchTargetType[] _collisionBodySearchTypes;
        [SerializeField]
        private CollisionBodyType _collisionBodyType = CollisionBodyType.Default;
        private ICollisionShape _collisionShape;

        #endregion Members

        #region Properties

        public int RefId { get; set; }
        public ICollisionShape CollisionShape => _collisionShape;
        public CollisionSearchTargetType[] CollisionSearchTargetTypes => _collisionBodySearchTypes;
        public Vector2 CollisionSystemPosition => transform.position;
        public Collider2D Collider => null;
        public CollisionBodyType CollisionBodyType => _collisionBodyType;

        public Action<CollisionResult, ICollisionBody> OnCollisionEvent { get; set;}

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