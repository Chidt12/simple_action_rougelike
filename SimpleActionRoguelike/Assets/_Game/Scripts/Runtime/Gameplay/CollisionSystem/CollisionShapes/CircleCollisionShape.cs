using System;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Gameplay.CollisionDetection
{
    public class CircleCollisionShape : ICollisionShape
    {
        #region Members

        private float _radius;
        private ICollisionBody _parent;

        #endregion Members

        #region Properties

        public float ExtentX => _radius;

        public float ExtentY => _radius;

        public double MaxExtent => _radius;

        public ICollisionBody Parent => _parent;

        public CollisionShapeType CollisionShapeType => CollisionShapeType.Circle;

        #endregion Properties

        #region Class Methods

        public CircleCollisionShape(ICollisionBody parent, float radius)
        {
            _radius = radius;
            _parent = parent;
        }

        public void DrawGizmos()
        {
            Gizmos.DrawSphere(Parent.CollisionSystemPosition, _radius + 0.1f);
        }

        public CollisionResult TestCollision(ICollisionShape other)
        {
            var result = new CollisionResult();

            if (other.CollisionShapeType == CollisionShapeType.Collider)
            {
                var contactFilter = new ContactFilter2D();
                contactFilter.useTriggers = other.Parent.Collider.isTrigger;
                var colliders = new List<Collider2D>();
                var numberColliders = Parent.Collider.OverlapCollider(contactFilter, colliders);
                if (numberColliders > 0)
                    result.collided = colliders.Contains(other.Parent.Collider);
                return result;
            }
            else
            {
                if (other.CollisionShapeType == CollisionShapeType.Rectangle)
                {
                    var otherPositionX = other.Parent.CollisionSystemPosition.x;
                    var otherPositionY = other.Parent.CollisionSystemPosition.y;
                    var ownerPositionX = Parent.CollisionSystemPosition.x;
                    var ownerPositionY = Parent.CollisionSystemPosition.y;

                    // Closest point in the rectangle to the center of circle
                    float closestX;
                    float closestY;

                    if (ownerPositionX > otherPositionX + other.ExtentX)
                        closestX = otherPositionX + other.ExtentX;
                    else if (ownerPositionX < otherPositionX - other.ExtentX)
                        closestX = otherPositionX - other.ExtentX;
                    else
                        closestX = ownerPositionX;

                    if (ownerPositionY > otherPositionY + other.ExtentY)
                        closestY = otherPositionY + other.ExtentY;
                    else if (ownerPositionY < otherPositionY - other.ExtentY)
                        closestY = otherPositionY - other.ExtentY;
                    else
                        closestY = ownerPositionY;

                    double a = Math.Abs(ownerPositionX - closestX);
                    double b = Math.Abs(ownerPositionY - closestY);
                    var distance = Math.Sqrt(a * a + b * b);

                    result.collided = distance <= ExtentX; // distance <= radius.
                }
                else
                {
                    var center = Parent.CollisionSystemPosition;
                    var otherCenter = other.Parent.CollisionSystemPosition;

                    if (Math.Abs(otherCenter.x - center.x) <= other.ExtentX + ExtentX)
                    {
                        if (Math.Abs(otherCenter.y - center.y) <= other.ExtentY + ExtentY)
                            result.collided = true;
                        else
                            result.collided = false;
                    }
                    else
                    {
                        result.collided = false;
                    }
                }

                return result;
            }
        }

        #endregion Class Methods
    }
}