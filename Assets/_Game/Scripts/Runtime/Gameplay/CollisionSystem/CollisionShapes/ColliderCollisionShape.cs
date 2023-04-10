using System;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Gameplay.CollisionDetection
{
    public class ColliderCollisionShape : ICollisionShape
    {
        #region Members

        private ICollisionBody _parent;

        #endregion Members

        #region Properties

        public ICollisionBody Parent => _parent;

        public float ExtentX => _parent.Collider.bounds.extents.x;

        public float ExtentY => _parent.Collider.bounds.extents.y;

        public double MaxExtent => Mathf.Max(ExtentX, ExtentY);
        public CollisionShapeType CollisionShapeType => CollisionShapeType.Collider;

        #endregion Properties

        #region Class Methods

        public ColliderCollisionShape(ICollisionBody parent)
            => _parent = parent;

        public void DrawGizmos() { }

        public CollisionResult TestCollision(ICollisionShape other)
        {
            var result = new CollisionResult();
            var contactFilter = new ContactFilter2D();
            contactFilter.useTriggers = other.Parent.Collider.isTrigger;
            var colliders = new List<Collider2D>();
            var numberColliders = Parent.Collider.OverlapCollider(contactFilter, colliders);
            if (numberColliders > 0)
                result.collided = colliders.Contains(other.Parent.Collider);
            return result;
        }

        #endregion Class Methods
    }
}