using Runtime.Gameplay.CollisionDetection;

namespace Runtime.Gameplay.EntitySystem
{
    public class CircleCollisionShapeBody : CollisionShapeBody
    {
        private float _radius;

        public void Init(float radius)
        {
            base.Init();
            _radius = radius;
        }

        public void UpdateRadius(float radius)
        {
            CollisionSystem.Instance.RemoveBody(this);
            Init(radius);
        }

        protected override ICollisionShape CreateShape()
        {
            var circleShape = new CircleCollisionShape(this, _radius);
            return circleShape;
        }
    }

}