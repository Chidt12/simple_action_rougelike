using Cysharp.Threading.Tasks;
using Runtime.Gameplay.CollisionDetection;
using System.Threading;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public class EntityCollideBehavior : EntityBehavior, ICollisionBody, IDisposeEntityBehavior
    {
        #region Members

        protected ICollisionShape collisionShape;
        private Collider2D _collider;
        protected CollisionSearchTargetType[] collisionBodySearchTypes;
        protected CollisionBodyType collisionBodyType;

        #endregion Members

        #region Properties

        public int RefId { get; set; }
        public ICollisionShape CollisionShape => collisionShape;
        public Vector2 CollisionSystemPosition => _collider.bounds.center;
        public CollisionSearchTargetType[] CollisionSearchTargetTypes => collisionBodySearchTypes;
        public Collider2D Collider => _collider;
        public CollisionBodyType CollisionBodyType => collisionBodyType;

        #endregion Properties

        #region Class Methods

        public async override UniTask<bool> BuildAsync(IEntityData data, CancellationToken cancellationToken)
        {
            await base.BuildAsync(data, cancellationToken);
            collisionBodySearchTypes = data.EntityType.GetCollisionBodySearchTypes(false);
            collisionBodyType = data.EntityType.GetCollisionBodyType();
            var collider = transform.GetComponent<Collider2D>();
            if (collider != null)
            {
                this._collider = collider;
                collisionShape = this.CreateCollisionShape(collider);
                CollisionSystem.Instance.AddBody(this);
                return true;
            }
            return false;
        }

        public virtual void OnCollision(CollisionResult result, ICollisionBody other) { }

        public void Dispose()
        {
            CollisionSystem.Instance.RemoveBody(this);
        }

        #endregion Class Methods
    }
}
