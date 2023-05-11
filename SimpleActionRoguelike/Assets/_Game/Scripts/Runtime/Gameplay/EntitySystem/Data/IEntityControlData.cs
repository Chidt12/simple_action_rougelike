using Runtime.Definition;
using System;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public interface IEntityControlData : IEntityData
    {
        Action<ActionInputType> PlayActionEvent { get; set; }
        Action MovementChangedEvent { get; set; }
        Action MovementUpdatedValueEvent { get; set; }
        Action DirectionChangedEvent { get; set; }
        public Vector2 MoveDirection { get; }
        public bool IsMoving { get; }
        public Vector2 FaceDirection { get; }
        public IEntityData Target { get; } // TODO: Put here properly ?
        void SetMoveDirection(Vector2 direction);
        void SetFaceDirection(Vector2 faceDirection);
        void SetTarget(IEntityData taret);
    }
}
