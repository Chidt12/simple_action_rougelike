using System;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Gameplay.CollisionDetection
{
    public class RectangleCollisionShape : ICollisionShape
    {
        #region Members

        private float _width;
        private float _height;
        private ICollisionBody _parent;

        #endregion Members

        #region Properties

        public float ExtentX => _width / 2;

        public float ExtentY => _height / 2;

        public double MaxExtent => Math.Sqrt(ExtentX * ExtentX + ExtentY * ExtentY);

        public ICollisionBody Parent => _parent;

        public CollisionShapeType CollisionShapeType => CollisionShapeType.Rectangle;

        #endregion Properties

        #region Class Methods

        public RectangleCollisionShape(ICollisionBody parent, float width, float height)
        {
            _width = width;
            _height = height;
            _parent = parent;
        }

        public void DrawGizmos()
            => Gizmos.DrawCube(Parent.CollisionSystemPosition, new Vector3(_width, _height, 0.1f));

        public CollisionResult TestCollision(ICollisionShape other)
        {
            var result = new CollisionResult();

            var center = Parent.CollisionSystemPosition;
            var otherCenter = other.Parent.CollisionSystemPosition;

            if (other.CollisionShapeType == CollisionShapeType.Collider)
            {
                var contactFilter = new ContactFilter2D();
                var colliders = new List<Collider2D>();
                contactFilter.useTriggers = other.Parent.Collider.isTrigger;
                var numberColliders = Parent.Collider.OverlapCollider(contactFilter, colliders);
                if (numberColliders > 0)
                    result.collided = colliders.Contains(other.Parent.Collider);
                return result;
            }
            else
            {
                if(other.CollisionShapeType == CollisionShapeType.Circle)
                {
                    var otherPositionX = other.Parent.CollisionSystemPosition.x;
                    var otherPositionY = other.Parent.CollisionSystemPosition.y;
                    var ownerPositionX = Parent.CollisionSystemPosition.x;
                    var ownerPositionY = Parent.CollisionSystemPosition.y;

                    // Closest point in the rectangle to the center of circle
                    float closestX;
                    float closestY;

                    if (otherPositionX > ownerPositionX + ExtentX)
                        closestX = ownerPositionX + ExtentX;
                    else if (otherPositionX < ownerPositionX - ExtentX)
                        closestX = ownerPositionX - ExtentX;
                    else
                        closestX = otherPositionX;

                    if (otherPositionY > ownerPositionY + ExtentY)
                        closestY = ownerPositionY + ExtentY;
                    else if (otherPositionY < ownerPositionY - ExtentY)
                        closestY = ownerPositionY - ExtentY;
                    else
                        closestY = otherPositionY;

                    double a = Math.Abs(otherPositionX - closestX);
                    double b = Math.Abs(otherPositionY - closestY);
                    var distance = Math.Sqrt(a * a + b * b);

                    result.collided = distance <= other.ExtentX; // distance <= radius.
                }
                else
                {
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