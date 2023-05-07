using Runtime.Definition;
using System;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public partial class EntityModel : IEntityControlData
    {
        protected Vector2 moveDirection;
        protected bool isMoving;
        protected Vector2 faceDirection;
        protected Vector2 moveDelta;
        protected IEntityData target;

        public Vector2 MoveDirection => moveDirection;
        public Vector2 FaceDirection => faceDirection;
        public bool IsMoving => isMoving;
        public Action<ActionInputType> PlayActionEvent { get; set; }
        public Action MovementChangedEvent { get; set; }
        public Action MovementUpdatedValueEvent { get; set; }
        public Action DirectionChangedEvent { get; set; }

        public IEntityData Target => target;

        protected void InitControl()
        {
            PlayActionEvent = _ => { };
            MovementChangedEvent = () => { };
            DirectionChangedEvent = () => { };
            MovementUpdatedValueEvent = () => { };
        }

        public void SetTarget(IEntityData positionData)
        {
            target = positionData;
        }

        public void SetMoveDirection(Vector2 value)
        {
            var direction = value;
            if (moveDirection != value)
            {
                moveDirection = value;
                MovementUpdatedValueEvent.Invoke();
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