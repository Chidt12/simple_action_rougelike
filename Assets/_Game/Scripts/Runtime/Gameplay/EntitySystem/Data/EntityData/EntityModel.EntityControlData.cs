using System;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public partial class EntityModel : IEntityControlData
    {
        protected Vector2 moveDirection;
        protected bool isMoving;
        protected Vector2 faceDirection;
        protected IEntityPositionData target;

        public Vector2 MoveDirection => moveDirection;
        public Vector2 FaceDirection => faceDirection;
        public Action<ActionInputType> ActionTriggeredEvent { get; set; }
        public Action MovementChangedEvent { get; set; }
        public Action DirectionChangedEvent { get; set; }

        public IEntityPositionData Target => target;

        protected void InitControl()
        {
            ActionTriggeredEvent = _ => { };
            MovementChangedEvent = () => { };
            DirectionChangedEvent = () => { };
        }

        public void SetMoveDirection(Vector2 value)
        {
            var direction = value;

            if (moveDirection != value)
            {
                moveDirection = value;
                if (isMoving && moveDirection == Vector2.zero)
                {
                    isMoving = false;
                    MovementChangedEvent.Invoke();
                }
                else if (!isMoving && moveDirection != Vector2.zero)
                {
                    isMoving = true;
                    MovementChangedEvent.Invoke();
                }
            }
        }

        public void SetFaceDirection(Vector2 faceDirection)
        {
            if(this.faceDirection != faceDirection)
            {
                this.faceDirection = faceDirection;
                DirectionChangedEvent?.Invoke();
            }
        }
    }
}