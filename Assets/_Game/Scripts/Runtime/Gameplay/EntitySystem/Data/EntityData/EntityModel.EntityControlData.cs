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
        protected IEntityPositionData target;

        public Vector2 MoveDirection => moveDirection;
        public Vector2 FaceDirection => faceDirection;
        public Vector2 MoveDelta => moveDelta;
        public bool IsMoving => isMoving;
        public Action<ActionInputType> PlayActionEvent { get; set; }
        public Action MovementChangedEvent { get; set; }
        public Action DirectionChangedEvent { get; set; }

        public IEntityPositionData Target => target;

        protected void InitControl()
        {
            PlayActionEvent = _ => { };
            MovementChangedEvent = () => { };
            DirectionChangedEvent = () => { };
        }

        public void SetTarget(IEntityPositionData positionData)
        {
            target = positionData;
        }

        public void SetMoveDirection(Vector2 value)
        {
            var direction = value;

            if (moveDelta != Vector2.zero)
                moveDelta = Vector2.zero;

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

        public void SetMoveDelta(Vector2 value)
        {
            if (moveDirection != Vector2.zero)
                moveDirection = Vector2.zero;

            if (moveDelta != value)
            {
                moveDelta = value;
                if (isMoving && moveDelta == Vector2.zero)
                {
                    isMoving = false;
                    MovementChangedEvent.Invoke();
                }
                else if (!isMoving && moveDelta != Vector2.zero)
                {
                    isMoving = true;
                    MovementChangedEvent.Invoke();
                }
            }
        }
    }
}