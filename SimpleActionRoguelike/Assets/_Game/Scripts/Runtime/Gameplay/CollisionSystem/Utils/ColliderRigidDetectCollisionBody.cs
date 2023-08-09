using UnityEngine;

namespace Runtime.Gameplay.CollisionDetection
{
    // It created out of the system, just in this case.

    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class ColliderRigidDetectCollisionBody : MonoBehaviour, ICollisionBody
    {
        [SerializeField]
        private Collider2D _collider;

        public int RefId { get; set; } = -100;

        public ICollisionShape CollisionShape => null;

        public CollisionSearchTargetType[] CollisionSearchTargetTypes => null;

        public CollisionBodyType CollisionBodyType => CollisionBodyType.Default;

        public Collider2D Collider => _collider;

        public Vector2 CollisionSystemPosition => transform.position;

        public void OnCollision(CollisionResult result, ICollisionBody other)
        {
        }

        //private void OnTriggerEnter2D(Collider2D collision)
        //{
        //    var collisionBody = collision.GetComponent<ICollisionBody>();

        //    if(collisionBody != null)
        //    {
        //        var collisionResult = new CollisionResult();
        //        collisionResult.collided = true;
        //        collisionResult.collisionType = CollisionType.Enter;
        //        collisionBody.OnCollision(collisionResult, this);
        //    }
        //}

        //private void OnTriggerExit2D(Collider2D collision)
        //{
        //    var collisionBody = collision.GetComponent<ICollisionBody>();

        //    if (collisionBody != null)
        //    {
        //        var collisionResult = new CollisionResult();
        //        collisionResult.collided = true;
        //        collisionResult.collisionType = CollisionType.Exit;
        //        collisionBody.OnCollision(collisionResult, this);
        //    }
        //}
    }
}
