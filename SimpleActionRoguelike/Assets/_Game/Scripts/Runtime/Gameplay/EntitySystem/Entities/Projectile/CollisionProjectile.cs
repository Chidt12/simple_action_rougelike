using Cysharp.Threading.Tasks;
using Runtime.Gameplay.CollisionDetection;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    [RequireComponent(typeof(Collider2D))]
    public class CollisionProjectile : Projectile, ICollisionBody
    {
        private ICollisionShape _collisionShape;
        private Collider2D _collider;
        private CollisionSearchTargetType[] _collisionBodySearchTypes;

        public int RefId { get; set; }
        public Vector2 CollisionSystemPosition => _collider.bounds.center;
        public ICollisionShape CollisionShape => _collisionShape;
        public CollisionSearchTargetType[] CollisionSearchTargetTypes => _collisionBodySearchTypes;
        public Collider2D Collider => _collider;
        public CollisionBodyType CollisionBodyType => CollisionBodyType.Projectile;

        private void OnDisable()
            => Dispose();

        public override async UniTask BuildAsync(IEntityData entityData, Vector3 position)
        {
            _collider = gameObject.GetComponent<Collider2D>();
            _collider.enabled = false;

            await base.BuildAsync(entityData, position);

            HasDisposed = false;
            RefId = -1;
            _collisionBodySearchTypes = entityData.EntityType.GetCollisionBodySearchTypes(true);
            _collisionShape = this.CreateCollisionShape(_collider);
            CollisionSystem.Instance.AddBody(this);
        }

        public override void InitStrategy(IProjectileStrategy projectileStrategy)
        {
            base.InitStrategy(projectileStrategy);
            _collider.enabled = true;
        }

        public void OnCollision(CollisionResult result, ICollisionBody other)
        {
            if (result.collided && result.collisionType == CollisionType.Enter && other.Collider != null)
                currentStrategy?.Collide(other.Collider);
        }

        public override void Dispose()
        {
            if (!HasDisposed)
            {
                HasDisposed = true;
                CollisionSystem.Instance.RemoveBody(this);
            }
        }
    }
}